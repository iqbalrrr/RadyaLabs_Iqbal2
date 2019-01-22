using System;
using System.ComponentModel.DataAnnotations;
using System.Security.Principal;
using System.Web;

namespace RadyaLabs.Objects
{
    public abstract class BaseView2
    {
        [Key]
        public virtual Int32 Id
        {
            get;
            set;
        }

        public virtual DateTime CreationDate
        {
            get
            {
                if (!IsCreationDateSet)
                    CreationDate = DateTime.Now;

                return InternalCreationDate;
            }
            protected set
            {
                IsCreationDateSet = true;
                InternalCreationDate = value;
            }
        }
        private Boolean IsCreationDateSet
        {
            get;
            set;
        }
        private DateTime InternalCreationDate
        {
            get;
            set;
        }

        public virtual string CreationUser
        {
            get
            {
                return CreationUser = HttpContext.Current.User.Identity.Name;
            }

            set
            {
                CreationUser = value;
            }
        }

        public virtual DateTime UpdateDate
        {
            get
            {
                return UpdateDate = DateTime.Now;
            }

            set
            {
                UpdateDate = value;
            }
        }

        public virtual string UpdateUser
        {
            get
            {
                return UpdateUser = HttpContext.Current.User.Identity.Name;
            }

            set
            {
                UpdateUser = value;
            }
        }
    }
}
