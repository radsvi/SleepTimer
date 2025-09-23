using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SleepTimer.Models
{
    public class QuestionPrompt
    {
        public async Task<bool> Show(string title, string message, string accept, string cancel)
        {
            var answer = await App.Current!.Windows[0].Page!.DisplayAlert(title, message, accept, cancel);

            return answer;
        }
    }
}
