namespace GraduationProjectModels
{
    public class BlankType: IEntity<BlankType>
    {
        public long Id { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }

        public void Edit(BlankType param)
        {
            Type = param.Type;
            Name = param.Name;
        }

        public bool Validate() => !string.IsNullOrWhiteSpace(Type) && !string.IsNullOrWhiteSpace(Name);
    }
}
