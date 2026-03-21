using Bunit;
using LibrarySystem.Web.Components.Layout;

namespace LibrarySystem.Tests;

public class NavMenuTests : TestContext
{
    [Fact]
    public void NavMenu_ShouldContainMainLinks()
    {
        var cut = RenderComponent<NavMenu>();

        var markup = cut.Markup;

        Assert.Contains("Startsida", markup);
        Assert.Contains("Böcker", markup);
        Assert.Contains("Medlemmar", markup);
        Assert.Contains("Lån", markup);
    }
}