#load "BasicForm.csx"

using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.FormFlow;
using Microsoft.Bot.Connector;
using System.Text;
using Newtonsoft.Json;

/// This dialog is the main bot dialog, which will call the Form Dialog and handle the results
[Serializable]
public class MainDialog : IDialog<BasicForm>
{
    public MainDialog()
    {
    }

    public Task StartAsync(IDialogContext context)
    {
        context.Wait(MessageReceivedAsync);
        return Task.CompletedTask;
    }

    public virtual async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> argument)
    {
        var message = await argument;
        context.Call(BasicForm.BuildFormDialog(FormOptions.PromptInStart), FormComplete);
    }

    private async Task FormComplete(IDialogContext context, IAwaitable<BasicForm> result)
    {
        try
        {
            var form = await result;
            if (form != null)
            {
                //post form to url
                var response = await SendJobForProbability(form);
            

                await context.PostAsync($"According to the ML algorithm, there is a {response}% probability that i'll be interested in the role.");
                
                if(Convert.ToInt32(response)<60){
                    await context.PostAsync($"Sorry :( (less that the 60% threshold)");
                    await context.PostAsync($"There won't be any one single factor affecting it's decision, but rest assured it's accuracy is very high.  Thank you for your time, and good luck with your search.");
                }
                    
            }
            else
            {
                await context.PostAsync("Form returned empty response! Type anything to restart it.");
            }
        }
        catch (OperationCanceledException)
        {
            await context.PostAsync("You canceled the form! Type anything to restart it.");
        }

        context.Wait(MessageReceivedAsync);
    }
    
    public async Task<string> SendJobForProbability(BasicForm form)
    {
        using (var client = new HttpClient())
        {
            string json = JsonConvert.SerializeObject(form);
            var requestData = new StringContent(json, Encoding.UTF8, "application/json");
    
            //You need to add an environment variable to your logic app.
            string logicAppsUrl = GetEnvironmentVariable("ROLESUBMISSIONAPI");

            var response = await client.PostAsync(logicAppsUrl, requestData);
            var result = await response.Content.ReadAsStringAsync();
    
            return result;
        }
    }
    
    public static string GetEnvironmentVariable(string name)
    {
        return Environment.GetEnvironmentVariable(name);
    
        //return name + ": " +
        //    System.Environment.GetEnvironmentVariable(name, EnvironmentVariableTarget.Process);
    }
}