using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextAdventure.Command.Determination.Result.Models;

namespace TextAdventure.Command.Determination.Intention
{
    public interface IBaseIntention
    {
        void PopulateResultModel(IResultModel resultModel);
    }
}
