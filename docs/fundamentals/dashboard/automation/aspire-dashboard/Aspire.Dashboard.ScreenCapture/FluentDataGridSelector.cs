using System.Text;

namespace Aspire.Dashboard.ScreenCapture;

public class FluentDataGridSelector(string initialSelector)
{
    private readonly StringBuilder _selector = new(initialSelector);

    public static FluentDataGridSelector Grid => new(".fluent-data-grid");

    public FluentDataGridSelector Head
    {
        get
        {
            _selector.Append(" thead");
            return this;
        }
    }

    public FluentDataGridSelector Body
    {
        get
        {
            _selector.Append(" tbody");
            return this;
        }
    }

    public FluentDataGridSelector Row(int index)
    {
        _selector.AppendFormat(" tr:nth-child({0})", index);
        return this;
    }

    public FluentDataGridSelector Cell(int index)
    {
        _selector.AppendFormat(" td:nth-child({0})", index);
        return this;
    }

    public FluentDataGridSelector Descendant(string selector)
    {
        _selector.AppendFormat(" {0}", selector);
        return this;
    }

    public override string ToString() => _selector.ToString();

    public static implicit operator string(FluentDataGridSelector fluentDataGridSelector) =>
        fluentDataGridSelector.ToString();
}
