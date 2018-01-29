namespace Robot.DAL.Core
{
    public class BaseEntity<K>
    {
        public virtual K Id { get; set; }
    }
}
