﻿using Erazer.Framework.FrontEnd;
using Erazer.Web.ReadAPI.ViewModels.Events;

namespace Erazer.Web.ReadAPI.EventHandlers.Redux
{
    public class ReduxCommentAddedAction : ReduxAction<TicketCommentEventViewModel>
    {
        public ReduxCommentAddedAction(TicketCommentEventViewModel payload) : base(ReduxActionTypeConstants.AddTicketComment, payload)
        {
        }
    }
}
