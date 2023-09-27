mergeInto(LibraryManager.library, {

    _init: async function () {
        init();
    },


    _openURL: function (url) {
        openURL(Pointer_stringify(url));
    },


    _stringReturnValueFunction: function () {
        var returnStr = "bla";
        var bufferSize = lengthBytesUTF8(returnStr) + 1;
        var buffer = _malloc(bufferSize);
        stringToUTF8(returnStr, buffer, bufferSize);
        return buffer;
    }
});