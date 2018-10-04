namespace GraduationProjectModels
{
    public class QuestionEntity: IEntity<QuestionEntity>
    {
        public long Id { get; set; }
        public long BlankTypeId { get; set; }
        public string Question { get; set; }

        public void Edit(QuestionEntity param)
        {
            Question = param.Question;
        }

        public bool Validate() => !string.IsNullOrWhiteSpace(Question);
    }
}
