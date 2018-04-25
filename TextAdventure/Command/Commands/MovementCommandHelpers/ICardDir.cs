using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextAdventure.Game.Game.Triggers;

namespace TextAdventure.Game.Command.Commands.MovementCommandHelpers
{
    public interface ICardDir
    {
        ITrigger FetchRoomTrigger { get; }

        string GetUnavailableDirDisp { get; }

        string GetDirExitBlockedDisp { get; }

        bool GetExitAllowedMod { get; }

        bool GetFastExitRequiredMod { get; }

        bool GetSlowExitRequiredMod { get; }

        string GetExitNotFastEnoughDisp { get; }

        string GetExitNotSlowEnoughDisp { get; }

        string GetNormalSpeedNotAllowedDisp { get; }

        bool GetJumpExitRequiredMod { get; }

        bool GetCrawlExitRequiredMod { get; }

        string GetExitMustJumpDisp { get; }

        string GetExitMustCrawlDisp { get; }

        string GetWalkingMethodNotAllowedDisp { get; }

        string GetExitOpenDisp { get; }
    }
}
