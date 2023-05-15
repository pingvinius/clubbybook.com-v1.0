namespace ClubbyBook.Common.Enums
{
    using System.ComponentModel;

    public enum ErrorCodeType : int
    {
        [Description("Неизвестная ошибка. Попробуйте перезагрузить страницу.")]
        [UrlRewrite("none")]
        None = 0,

        [Description("Выключен JavaScript.")]
        [UrlRewrite("disabled-javascript")]
        DisabledJavaScript = 1,

        [Description("Ваш браузер не поддерживается.")]
        [UrlRewrite("unsupported-browser")]
        UnsupportedBrowser = 2,

        [Description("Удаление учетной записи не удалось.")]
        [UrlRewrite("removing-user-account-failed")]
        RemovingUserAccountFailed = 3,

        [Description("Произошли проблемы с UrlRewrite.")]
        [UrlRewrite("invalid-url-rewrite")]
        InvalidUrlRewrite = 4,

        [Description("У Вас недостаточно прав для выполнения этой операции.")]
        [UrlRewrite("unauthorized-access")]
        UnauthorizedAccess = 5,

        [Description("Произошла не предвиденная ошибка. Попробуйте зайти на страницу позже.")]
        [UrlRewrite("unexpected-error")]
        UnexpectedErrorHasOccurred = 6,
    }
}