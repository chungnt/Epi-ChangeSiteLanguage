using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EPiServer.DataAbstraction;
using EPiServer.Web;

namespace ChangeSiteLang.ViewModels
{
    public class SiteLangViewModel
    {
        public SiteDefinition Site { get; set; }
        public LanguageBranch LanguageBranche { get; set; }
    }
}
