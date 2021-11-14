using Siemens.Simatic.S7.Webserver.API.Enums;

namespace Siemens.Simatic.S7.Webserver.API.Services.RequestHandling
{
    public interface IRequestParameterChecker
    {
        void CheckETag(string etag, bool performCheck);
        void CheckLastModified(string lastModified, bool performCheck);
        void CheckMediaType(string mediaType, bool performCheck);
        void CheckPlcProgramBrowseMode(ApiPlcProgramBrowseMode plcProgramBrowseMode, bool performCheck);
        void CheckPlcProgramReadOrWriteMode(ApiPlcProgramReadOrWriteMode? apiPlcProgramReadMode, bool performCheck);
        void CheckPlcProgramWriteOrReadDataType(ApiPlcProgramDataType apiPlcProgramData, bool performCheck);
        void CheckPlcRequestChangeOperatingMode(ApiPlcOperatingMode plcOperatingMode, bool performCheck);
        void CheckResourceName(string resourceName, bool performCheck);
        void CheckState(ApiWebAppState apiWebAppState, bool performCheck);
        void CheckTicket(string ticketId, bool performCheck);
        void CheckVisibility(ApiWebAppResourceVisibility apiWebAppResourceVisibility, bool performCheck);
        void CheckWebAppName(string webAppName, bool performCheck);
    }
}