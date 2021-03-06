﻿namespace Erazer.Domain.Data.DTOs.Events
{
    public class PriorityEventDto : TicketEventDto
    {
        public PriorityDto FromPriority { get; private set; }
        public PriorityDto ToPriority { get; private set; }

        public PriorityEventDto(PriorityDto from, PriorityDto to)
        {
            FromPriority = from;
            ToPriority = to;
        }
    }
}
