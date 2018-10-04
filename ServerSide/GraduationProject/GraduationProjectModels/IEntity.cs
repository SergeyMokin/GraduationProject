namespace GraduationProjectModels
{
    // Interface of class what contains in database.
    public interface IEntity<in T>
    {
        long Id { get; set; }

        bool Validate();

        void Edit(T param);
    }
}
