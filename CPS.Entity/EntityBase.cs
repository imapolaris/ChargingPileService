using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPS.Entities
{
    public class EntityBase
    {
        public EntityBase()
        {
            this.id = Guid.NewGuid().ToString();
        }

        [Key, Column(Order = 1), StringLength(50)]
        private string id;
        public virtual string Id
        {
            get { return id; }
            set { id = value; }
        }

        private byte del;
        /// <summary>
        /// 删除标识
        /// </summary>
        [NotMapped]
        public byte Del
        {
            get { return del; }
            set { del = value; }
        }

    }
}
