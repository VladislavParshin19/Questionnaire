using System.Collections.Generic;

namespace Questionnaire
{
    class QuestionTemplate
    {
        public string Category { get; set; }
        public string QuestionText { get; set; }
        public List<string> Blanks { get; set; }
    }
}
