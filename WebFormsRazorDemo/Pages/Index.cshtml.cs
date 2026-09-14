using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

public class IndexModel : PageModel
{
    [BindProperty]
    public string? TxtName { get; set; }


    public string? LblResult { get; set; }


    public string LitLog { get; set; } = "";


    public string ViewStateDump { get; set; } = "";


    public IndexModel()
    {
        LitLog += "Init (конструктор модели)<br/>";
    }


    public void OnGet()
    {
        LitLog += "Load (OnGet)<br/>";
    }

    public void OnPost()
    {
        LitLog += "Load (OnPost)<br/>";
    }


    public void OnPostHello()
    {
        LitLog += "Postback (OnPostHello)<br/>";
        if (!string.IsNullOrWhiteSpace(TxtName))
            LblResult = $"Привет, {TxtName}!";
    }


    public IActionResult OnPostClear()
    {
        LitLog += "Postback (OnPostClear)<br/>";
        TxtName = null;
        LblResult = null;

        return RedirectToPage();
    }


    public override void OnPageHandlerExecuted(
        Microsoft.AspNetCore.Mvc.Filters.PageHandlerExecutedContext context)
    {
        LitLog += "PreRender (OnPageHandlerExecuted)<br/>";
        ViewStateDump = DumpHiddenFields();
        base.OnPageHandlerExecuted(context);
    }

    private string DumpHiddenFields()
    {
        if (Request.HasFormContentType && Request.Form.Count > 0)
        {
            var sb = new System.Text.StringBuilder();
            foreach (var key in Request.Form.Keys)
            {
                if (key.StartsWith("__"))
                    sb.AppendLine($"{key} = {Request.Form[key]} (длина: {Request.Form[key].ToString().Length})");
            }
            return sb.Length > 0 ? sb.ToString() : "Скрытых полей __* нет";
        }
        return "Это GET-запрос — скрытых полей нет";
    }
}