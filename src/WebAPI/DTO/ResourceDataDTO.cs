﻿namespace WebAPI.DTO
{
    public class ResourceDataRespIDValueDTO
    {
        public int AttributeId { get; set; } //foreignKey ==>partial Key

        public string AttributeValue { get; set; }

    }

    public class ResourceDataRespDTO
    {
        public int ResourceId { get; set; } //foreignKey ==>partial Key
        public int AttributeId { get; set; } //foreignKey ==>partial Key

        public string AttributeValue { get; set; }


    }
}
