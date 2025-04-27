using FainEngine_v2.Core;
using FainEngine_v2.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FainEngine_v2.Entities
{
    public class UIController : IEntity
    {
        UIManager manager;
        public UIController() 
        { 
            manager = new UIManager();
            GameGraphics.SetUIManager(manager);
        }
    }
}
