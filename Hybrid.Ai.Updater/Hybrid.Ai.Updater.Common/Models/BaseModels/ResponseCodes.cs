namespace Hybrid.ai.Geoposition.Common.Models.BaseModels
{
    public enum ResponseCodes
    {
        SUCCESS = 0,
        DATABASE_ERROR = 1,
        USER_NOT_FOUND = 2,
        USER_CREDENTIALS_IS_WRONG = 3,
        NOT_FOUND_RECORDS = 4,
        DATABASE_TIME_OUT = 5,
        INVALID_PARAMETER = 6,
        FILE_NOT_SAVED = 7,
        WRONG_LANG_CODE = 8,
        RECORD_ALREADY_EXIST = 9,
        TECHNICAL_ERROR = 10,
        USER_LOGIN_ALREADY_EXISTS = 11,
        FILE_NOT_DOWNLOADED = 12,
        FAILURE = 13,
        INTERNAL_BAD_REQUEST = 15,
        MESSAGE_NOT_SENT = 16,
        FILE_SO_BIG = 18,
        SOME_OR_ALL_ENTITIES_WERE_NOT_UPDATED = 20,
        ACCESS_DENIED = 21
    }
}