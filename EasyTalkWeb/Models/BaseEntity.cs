﻿namespace EasyTalkWeb.Models
{
    public abstract class BaseEntity 
    {
        public Guid Id { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime? ModifiedDate { get; set; }
    }

    //public interface IBaseEntity
    //{
    //    public Guid Id { get; set; }

    //    public DateTime CreatedDate { get; set; }

    //    public DateTime? ModifiedDate { get; set; }
    //}
}
