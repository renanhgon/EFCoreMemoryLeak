namespace Domain.Entities.Base.ByUser
{
    public abstract class EntityByUser : Entity
    {
        public User? CreatedBy { get; private set; }

        public Guid CreatedByUserId { get; private set; }

        public User? UpdatedBy { get; private set; }

        public Guid? UpdatedByUserId { get; private set; }

        public EntityByUser SetCreatedByUser(User user)
        {
            CreatedByUserId = user.Id;
            CreatedBy = user;
            return this;
        }

        public EntityByUser SetUpdatedByUser(User user)
        {
            UpdatedBy = user;
            UpdatedByUserId = user?.Id;
            return this;
        }
    }
}