namespace SurveyBasket.Helpers;

public static  class EmailBodyBuilder
{
    public static string GenerateEmailBody(string template, Dictionary<string, string> templateModel)
    {
        var templatePath =  Path.Combine(
    Directory.GetCurrentDirectory(),
    "Templates",
    $"{template}.html"
);
        StreamReader reader = new StreamReader(templatePath);
        var body = reader.ReadToEnd();
        reader.Close();

        foreach(var item in templateModel)
        {
            body = body.Replace(item.Key, item.Value);

        };
        return body;
    }
}
