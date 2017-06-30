using System;
using Microsoft.Bot.Builder.FormFlow;

public enum JobTitle { Solutions_Architect = 1, Chief_Architect, CTO, Lead_Developer};
public enum CompanyScale { Global_Brand = 1, UK_Brand, SME};
public enum CompanyType { Cloud_Vendor = 1, Cloud_Partner, Consultancy, Company };
public enum CloudVendor { AWS = 1, Google, Microsoft, Oracle, Other };



// For more information about this template visit http://aka.ms/azurebots-csharp-form
[Serializable]
public class BasicForm
{
    [Prompt("What's your email address")]
    public string Email { get; set; }
    
    [Prompt("Please select the closest match for the job title {||}")]
    public JobTitle Title { get; set; }
    
    [Prompt("In pounds, approximately how much is the base salary excluding bonus and benefits {||}?")]
    public int Salary { get; set;}
    
    [Prompt("What sort of company is the job for {||}")]
    public CompanyType CorpType { get; set; }
    
    [Prompt("Which cloud vendor {||}")]
    public CloudVendor CloudCompany { get; set; }
    
    [Prompt("What size of company is it {||}")]
    public CompanyScale Size { get; set; }

    public static IForm<BasicForm> BuildForm()
    {
        // Builds an IForm<T> based on BasicForm
        //return new FormBuilder<BasicForm>().Build();
        

        
        return new FormBuilder<BasicForm>()
                    .Message("Lets get started.")
                    .Field(nameof(Title))
                    .Field(nameof(Salary))
                    .Field(nameof(CorpType))
                    .Field(nameof(CloudCompany), (c) => c.CorpType == CompanyType.Cloud_Vendor)
                    .Field(nameof(Size))
                    .AddRemainingFields()
                    .Confirm("Are you sure? Here are your current selections: {*}")
                    .Build();
    }

    public static IFormDialog<BasicForm> BuildFormDialog(FormOptions options = FormOptions.PromptInStart)
    {
        // Generated a new FormDialog<T> based on IForm<BasicForm>
        return FormDialog.FromForm(BuildForm, options);
    }
}
