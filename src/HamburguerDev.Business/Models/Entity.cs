using System.ComponentModel.DataAnnotations;

namespace HamburguerDev.Business.Models
{
    public abstract class Entity
    {
        [Required(ErrorMessage ="O campo {0} é necessário")]
        public Guid Id { get; set; }

        protected Entity()
        {
            Id = Guid.NewGuid();
        }

        protected Entity(Guid id)
        {
            Id = id;
        }
    }
}
