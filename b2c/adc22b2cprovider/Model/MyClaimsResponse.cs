using SpectoLogic.Identity.AADB2C.APIConnectors;
// DO NOT USE namespace …; syntax as my api connector does not support that currently
namespace adc22b2cprovider.Model
{
    [ExtensionAppId("eff3e2b4-6308-437e-953f-95fec3dc1573")]
    [CustomClaim("ADCD_ID", typeof(string))]
    [CustomClaim("LoyaltiyID", typeof(string))]
    public partial class MyClaimsResponse
    {
    }
}

