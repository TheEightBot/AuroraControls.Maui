using Microsoft.Maui.Controls.Platform;
using UIKit;

namespace AuroraControls;

public class AppleListViewHideEmptyCellsEffect : PlatformEffect
{
    protected override void OnAttached()
    {
        if (this.Control is UITableView tableView && tableView.TableFooterView == null)
        {
            tableView.TableFooterView = new UIView();
        }
    }

    protected override void OnDetached()
    {
        if (this.Control is UITableView tableView)
        {
            tableView.TableFooterView = null;
        }
    }
}
