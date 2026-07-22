# Clear Button for Pickers — Design (Bucket A)

> Status: **Design / pending review.** No production code yet.
> Goal anchor: give Aurora a *standard, reusable contract* for "clear this input's value," conforming to MAUI's `Entry.ClearButtonVisibility` so it feels native. CalendarPicker is the first proof; a reusable Effect is the mechanism for other pickers.

## The contract we're conforming to

MAUI's clear button is governed by the **`Microsoft.Maui.ClearButtonVisibility`** enum (namespace `Microsoft.Maui`, used by `IEntry`/`Entry`):

| Member | Value | Meaning |
|--------|-------|---------|
| `Never` | 0 | Never show a clear button (Entry default). |
| `WhileEditing` | 1 | Show the clear "X" **only while the field has focus and a value**; tapping it clears the value. |

**Decision:** reuse this exact enum type — we do **not** invent an Aurora-specific one. "Behaves the same as Entry" is then literally true at the API level.

## Key finding: pickers are already text-field-backed

The reason this is feasible without overlaying a custom button: MAUI backs date/time/list pickers with the *same native substrate* Entry uses.

| Platform | Entry native view | DatePicker native view (`MauiDatePicker`) | Clear mechanism available |
|----------|-------------------|-------------------------------------------|---------------------------|
| iOS | `UITextField` | `NoCaretField : UITextField` | **Native** `UITextField.ClearButtonMode` |
| MacCatalyst | `UITextField` | `NoCaretField : UITextField` (shared iOS code) | **Native** `UITextField.ClearButtonMode` |
| Android | `AppCompatEditText` | `MauiDatePicker : AppCompatEditText` | Compound end-drawable + touch listener (same approach MAUI uses for Entry) |

How MAUI maps it (verified against `dotnet/maui` `main`):
- **iOS/Mac:** `UpdateClearButtonVisibility` sets `ClearButtonMode = WhileEditing` (else `Never`). The native clear empties the text, and MAUI propagates it back via the **`EditingChanged`** event (no `ShouldClear` delegate). → We do the same: on `EditingChanged` with empty text, set the bound value to null.
- **Android:** clear button = a compound drawable positioned at the trailing edge, shown only when `WhileEditing && hasText && isFocused`, with taps detected by hit-testing the drawable bounds in a touch handler.

> Windows is **out of scope** (CalendarPicker has no Windows handler), which conveniently sidesteps the known MAUI Windows clear-button bugs (dotnet/maui #25038, #25225, #25473).

## The one real design tension: affordance vs. clear-action

The *visual affordance* (render an X, show/hide it, detect the tap) **generalizes trivially** — every target is `UITextField`/`AppCompatEditText`. But **what "cleared" means is control-specific**:

| Control | "Cleared" = | Has a null state? |
|---------|-------------|-------------------|
| Aurora `CalendarPicker` | `Date = null` | ✅ already nullable |
| standard `DatePicker` | (no null — `Date` is non-nullable) | ❌ ill-defined |
| `Picker` | `SelectedIndex = -1` | ✅ |
| `TimePicker` | (no null) | ❌ |

This is why a generic Effect cannot *universally* know what to clear. The architecture below splits the two concerns.

## Recommended architecture

**Shared platform helper** — one place that knows how to render + intercept the clear on each platform (`ClearButtonMode` on iOS/Mac; drawable + touch on Android), exposing a single "clear requested" callback. Used by both surfaces below so there's no duplication.

**Surface 1 — CalendarPicker first-class property (v1, the proof).**
- Add `ClearButtonVisibility` **bindable property** to `CalendarPicker` (type `Microsoft.Maui.ClearButtonVisibility`, default `Never`).
- CalendarPicker already has custom per-platform handlers — the natural home. On "clear requested" → `Date = null` (the existing `ClearButtonOnClicked`/`TryShowEmptyState` plumbing already does exactly this).
- Discoverable, XAML-friendly, no attached property needed for the flagship control.

**Surface 2 — reusable Effect (Bucket F, the generalization).**
- `ClearButton` attached-property class + `ClearButtonEffect : RoutingEffect`, mirroring the existing `KeyboardToolbar` pattern (`AuroraControls.AttachedProperties`, `xmlns:ap`).
- Attached `ClearButton.Visibility` (the same enum) drives the affordance.
- Because the Effect can't know the clear semantics, it exposes a **`ClearButton.Command`** (and/or a `ClearButtonClicked` event) the consumer wires to define what "cleared" sets. It also empties the native text so the field looks cleared immediately.
- Validated against a plain `DatePicker`/`Picker` in Bucket F.

## Per-platform approach & risks

**iOS** (`Platforms/iOS/CalendarPickerHandler.cs`)
- Map `ClearButtonVisibility` → `PlatformView.ClearButtonMode`.
- Subscribe to `PlatformView.EditingChanged`; when text empties via the X → `VirtualView.Date = null`. (Programmatic `Text` sets don't raise `EditingChanged`, so an empty-text event reliably means "user tapped clear.")
- *Nuance:* the existing input-accessory toolbar already has an always-present **"Clear"** `UIBarButtonItem`. With `WhileEditing`, both appear only while editing, so they're redundant-but-harmless. → sub-decision below.

**MacCatalyst** (`Platforms/MacCatalyst/CalendarPickerHandler.cs`)
- Currently **stubbed/commented-out** — it does not carry the iOS nullable/clear logic today. Bucket D is real work: port the relevant handler logic (or factor a shared MaciOS partial) and apply the same `ClearButtonMode` mapping. `UITextField` APIs are available here, so the mechanism is identical to iOS.

**Android** (`Platforms/Android/CalendarPickerHandler.cs`)
- Add a trailing compound drawable (clear icon) on the `MauiDatePicker` (`AppCompatEditText`); toggle per `WhileEditing && hasValue && isFocused`; intercept taps via touch listener → `VirtualView.Date = null`.
- *Risks:* (1) the read-only date `EditText`'s focus model differs from a typed Entry — "WhileEditing" may need a pragmatic interpretation (e.g. focused/dialog-open). (2) Need a clear-icon drawable (system resource vs. a shipped vector). Both resolved in Bucket E.

## Locked decisions (confirmed)

1. **CalendarPicker API shape** — ✅ first-class **bindable property** `ClearButtonVisibility` on CalendarPicker, **plus** a separate attached-property Effect for other pickers. Shared platform helper underneath.
2. **Generic clear semantics** — ✅ the reusable Effect renders the X, empties native text, and delegates the actual clear via **`ClearButton.Command` (+ `ClearButtonClicked` event)** — consumer defines what "cleared" means.
3. **iOS redundancy** — ✅ keep the existing accessory-toolbar "Clear" button **and** add the inline X. No change to the toolbar in v1.
4. **WhileEditing UX** — ✅ ship `Never`/`WhileEditing` exactly like Entry; inline X visible **only while the picker is focused/open**. `Always` is explicitly out of v1 (may be added later, additively).

## What Bucket B will do (next)

- Add the `ClearButtonVisibility` bindable property to `CalendarPicker` (reusing `Microsoft.Maui.ClearButtonVisibility`, default `Never`).
- Stand up the reusable `ClearButton` attached property + `ClearButtonEffect : RoutingEffect` skeleton and the shared platform-helper seam.
- Wire the cross-platform clear→`null` contract for CalendarPicker. **No native rendering yet** — that's Buckets C/D/E.
