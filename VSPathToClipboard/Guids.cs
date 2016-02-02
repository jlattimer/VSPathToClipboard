using System;

namespace VSPathToClipboard
{
    static class GuidList
    {
        public const string GuidVsPtcPkgString = "717dba23-b8e2-4678-ab9d-4da3335dd4c2";
        public const string GuidVsPtcCmdSetString = "028e2d02-98bb-4e21-9208-0c57966493fe";
        public static readonly Guid GuidVsPtcCmdSet = new Guid(GuidVsPtcCmdSetString);
    };
}