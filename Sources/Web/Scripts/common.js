//////////////////////////////////////
// Constants
//////////////////////////////////////

var Keys =
{
    Backspace: 8,
    Tab: 9,
    Enter: 13,
    Escape: 27,
    Space: 32,
    PageUp: 33,
    PageDown: 34,
    End: 35,
    Home: 36,
    Left: 37,
    Up: 38,
    Right: 39,
    Down: 40,
    NumpadEnter: 108,
    Comma: 188
};

var Errors =
{
    ArgumentException: "Argument exception.",
    ArgumentNullException: "Argument NULL exception.",
    InvalidOperationException: "Invalid operation exception.",
    ArgumentOutOfRangeException: "Argument out of range exception."
};

//////////////////////////////////////
// Extends javascript framework
//////////////////////////////////////

String.prototype.toArrayList = function ()
{
    if ($(this).length !== 1)
    {
        throw Errors.InvalidOperationException; // Only one element could use this method.
    }

    var ids = new Array();
    $.each($(this)[0].toString().split(","), function (index, value)
    {
        if (value !== "")
            ids.push(parseInt(value, 10));
    });

    return ids;
};

Array.prototype.indexOf = function (searchValue, searchPredicate)
{
    if (!isDefinedAndNotNull(searchPredicate))
    {
        searchPredicate = function (val1, val2) { return val1 == val2; };
    }

    var foundIndex = -1;
    $.each(this, function (index, value)
    {
        if (searchPredicate(value, searchValue))
        {
            foundIndex = index;
            return;
        }
    });

    return foundIndex;
};

Array.prototype.removeAt = function (index)
{
    if (index < 0 && index >= this.length)
    {
        Errors.ArgumentOutOfRangeException;
    }

    return this.splice(index, 1);
};

Array.prototype.remove = function (value, searchPredicate)
{
    var index = this.indexOf(value, searchPredicate);
    if (index !== -1)
    {
        return this.removeAt(index);
    }

    return null;
};

String.prototype.format = function ()
{
    var args = arguments;
    return this.replace(/{(\d+)}/g, function (match, number)
    {
        return typeof args[number] != 'undefined' ? args[number] : match;
    });
};

//////////////////////////////////////
// Public methods
//////////////////////////////////////
function isDefined(variable)
{
    return typeof variable !== "undefined";
}

function isStringEmpty(variable)
{
    return typeof variable === "string" && variable === "";
}

function isDefinedAndNotNull(variable)
{
    return variable !== null && isDefined(variable);
}

function isVisible(uiElement)
{
    return uiElement !== null && $(uiElement).css("display") !== "none";
}

function isJSON(text)
{
    text = text.replace(/\\["\\\/bfnrtu]/g, '@');
    text = text.replace(/"[^"\\\n\r]*"|true|false|null|-?\d+(?:\.\d*)?(?:[eE][+\-]?\d+)?/g, ']');
    text = text.replace(/(?:^|:|,)(?:\s*\[)+/g, '');
    return (/^[\],:{}\s]*$/.test(text));
}

function getQueryParameters()
{
    var parameters = window.location.search.substr(1).split('&');
    if (parameters === "")
        return {};

    var variables = {};
    for (var index = 0; index < parameters.length; index++)
    {
        var pair = parameters[index].split('=');
        if (pair.length !== 2)
            continue;

        variables[pair[0].toLowerCase()] = decodeURIComponent(pair[1].replace(/\+/g, " "));
    }

    return variables;
}

function getDocumentScrollOffset()
{
    return { left: window.pageXOffset, top: window.pageYOffset };
}

function getDocumentSize()
{
    var documentWidth;
    var documentHeight;

    if (typeof window.innerWidth !== "undefined")
    {
        documentWidth = window.innerWidth,
        documentHeight = window.innerHeight
    }
    else if (typeof document.documentElement !== "undefined" &&
        typeof document.documentElement.clientWidth !== "undefined" &&
        document.documentElement.clientWidth != 0)
    {
        documentWidth = document.documentElement.clientWidth,
        documentHeight = document.documentElement.clientHeight
    }
    else
    {
        documentWidth = document.getElementsByTagName("body")[0].clientWidth,
        documentHeight = document.getElementsByTagName("body")[0].clientHeight
    }

    return { width: documentWidth, height: documentHeight };
}

function redirectToPage(url)
{
    if (!isDefinedAndNotNull(url))
        throw Errors.ArgumenNullException;

    document.location = url;
}

//////////////////////////////////////
// Executable code
//////////////////////////////////////

if (document.location.pathname.toLowerCase() === "/default.aspx")
{
    document.location.replace('/');
}