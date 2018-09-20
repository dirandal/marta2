#load "StaticUtils.csx"
using System;
using System.Collections.Generic;
using Microsoft.Bot.Builder.Dialogs;
using System.Threading.Tasks;
using Microsoft.Bot.Connector;


    [Serializable]
    public class FurtherInformationDialog : IDialog<string>
    {
        private int attempts = 1;

        // ENTITIES =======================================================
        private const string ModuleID = "ModuleID";
        private const string AssessmentNumber = "AssessmentNumber";
        private const string LectureType = "LectureType";
        private const string RessourceType = "RessourceType";
        private const string Week = "Week";
        private const string LecturerNameEntity = "LecturerName";

        public async Task StartAsync(IDialogContext context)
        {
            var reply = context.MakeMessage();
            

            reply.Text = $"Sorry, I did not understand. Which {entityFamiliarName} are you referring to?. Could you please specify a {entityRequest}? ";
            reply.Speak = $"Could you please specify a {entityRequest} ?";

            await context.PostAsync(reply);
            context.Wait(this.MessageReceivedAsync);
        }

        private string entityFamiliarName;
        private string entityRequest;
        private List<string> EntityID;





        public FurtherInformationDialog(List<string> EntityID, string entityFamiliarName, string entityRequest)
        {
            this.entityFamiliarName = entityFamiliarName;
            this.entityRequest = entityRequest;
            this.EntityID = EntityID;
        }


        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var message = await result;
            bool entityfound = false;
            //If the message returned contains an entity , return it to the calling dialog. 
            foreach (var entity in EntityID)//for each entity given by the calling dialog, try to find a corresponding value in user's answer
            {
                string foundEntity = null;
                string closestEntity = null;
                int confidence;
                if((message.Text.Contains("general")) || (message.Text.Contains("schedule")) || (message.Text.Contains("General")) || (message.Text.Contains("Schedule")))
                {
                    context.ConversationData.SetValue("value entity", "general schedule");
                    context.ConversationData.SetValue("name entity", entity);


                    entityfound = true;
                    //Completes the dialog, removes it from the dialog stack, and returns the result to the parent/calling dialog. 
                    context.Done(message.Text);
                    break;
                }
                else
                {
                    if (entity == ModuleID)
                        foundEntity = StaticUtils.getCorresponding(StaticUtils.Module, message.Text);
                    if (entity == LecturerNameEntity)
                    {
                        context.ConversationData.SetValue("name entity", entity);
                        StaticUtils.getCorresponding(StaticUtils.Lecturer, message.Text, out foundEntity, out closestEntity, out confidence);
                        if (foundEntity == null && confidence > 70)
                        {
                            foundEntity = closestEntity;
                        }
                    }
                    if (entity == LectureType)
                    {
                        context.ConversationData.SetValue("name entity", entity);
                        StaticUtils.getCorresponding(StaticUtils.LectureTypeDic, message.Text, out foundEntity, out closestEntity, out confidence);
                        if (foundEntity == null && confidence > 70)
                        {
                            foundEntity = closestEntity;
                        }
                    }
                    if (entity == AssessmentNumber)
                    {
                        context.ConversationData.SetValue("name entity", entity);
                        StaticUtils.getCorresponding(StaticUtils.AssessmentNumber, message.Text, out foundEntity, out closestEntity, out confidence);
                        if (foundEntity == null && confidence > 70)
                        {
                            foundEntity = closestEntity;
                        }
                    }
                    if (entity == Week)
                    {
                        context.ConversationData.SetValue("name entity", entity);
                        StaticUtils.getCorresponding(StaticUtils.Week, message.Text, out foundEntity, out closestEntity, out confidence);

                        if (foundEntity == null && confidence > 70)
                        {
                            foundEntity = closestEntity;

                        }

                    }

                    if ((message.Text != null) && (foundEntity != null))
                    {
                        //EntityList => entity list used to create the answer


                        //Update Query and Answer


                        //store the found entity in the conversational data
                        context.ConversationData.SetValue("value entity", foundEntity);
                        context.ConversationData.SetValue("name entity", entity);


                        entityfound = true;
                        //Completes the dialog, removes it from the dialog stack, and returns the result to the parent/calling dialog. 
                        context.Done(message.Text);
                        break;
                    }
                }
               
            }
            if (!entityfound)
            {

                {
                    if (--attempts > 0)
                    {
                        var reply = context.MakeMessage();

                        reply.Text = $"Could you please specify a {entityRequest}?";
                        reply.Speak = $"Could you please specify a {entityRequest} ?";

                        await context.PostAsync(reply);
                        context.Wait(this.MessageReceivedAsync);

                    }
                    else
                    {
                        //return a too many attempts to luisdialog
                        context.Done(StaticUtils.TooManyAttempts);
                    }
                }
            }
        }
    }
