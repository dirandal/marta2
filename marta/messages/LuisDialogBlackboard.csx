#load "StaticUtils.csx"
#load "FurtherInformationDialog.csx"

using System;
using System.Collections.Generic;
using Microsoft.Bot.Builder.Azure;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using System.Threading.Tasks;
using Microsoft.Bot.Connector;
using System.Data.SqlClient;
using AdaptiveCards;


    //[LuisModel("cc6de590-09b4-487b-84e5-051f4c863f3f", "3154f06716504c21962ae4b294a6a006")]
    [Serializable]
    public class LuisDialogBlackboard : LuisDialog<object>
    {
        
        public string key { get; private set; }
        public string module { get { return StaticUtils.ModuleKey[key]; } }
        public string CurrentIntend { get; set; }
        public string CurrentResourceType { get; set; }
        public Boolean isLectureType { get; set; }
        public string CurrentLectureType { get; set; }

        private string getCurrentModule(IDialogContext context)
        {
            return module;
        }
   
        public LuisDialogBlackboard(string key) : base(new LuisService(new LuisModelAttribute(Utils.GetAppSetting("LuisAppId"), Utils.GetAppSetting("LuisAPIKey"), domain: Utils.GetAppSetting("LuisAPIHostName"))))
        {
             CurrentLectureType = "";
            isLectureType = false;
            CurrentResourceType = "";
            this.key = key;
        }

        // ENTITIES =======================================================
        private const string ModuleID = "ModuleID";
        private const string AssessmentNumber = "AssessmentNumber";
        private const string LectureType = "LectureType";
        private const string RessourceType = "ResourceType";
        private const string Week = "Week";
        private const string LecturerNameEntity = "LecturerName";

        /// <summary>
        /// Function Triggered when a message is recevied.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="item"></param>
        /// <returns></returns>

        protected override async Task MessageReceived(IDialogContext context, IAwaitable<IMessageActivity> item)
        {
            /// <summary>
            /// dictionnary launch display function if user message equal dictionnary key 
            /// </summary>
            Dictionary<string, IMessageActivity> DisplayIntend = new Dictionary<string, IMessageActivity>
            {
                    {"Terms", Terms(context)},
                    {"terms", Terms(context)},
                    {"Privacy", Terms(context)},
                    {"privacy", Terms(context)},
            };
            var message = await item;
            if (message.Text == null)
            {
                await this.SendWelcomeMessageAsync(context);
            }
            // dictionnary launch display function if user message equal dictionnary key 
            else if (DisplayIntend.ContainsKey(message.Text))
            {

                await context.PostAsync(DisplayIntend[message.Text]);
                context.Wait(this.MessageReceived);
            }
            else
            {
                await base.MessageReceived(context, item);
            }
        }
        /// <summary>
        /// Send a Welcome message to the user, after that, message received will run
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private async Task SendWelcomeMessageAsync(IDialogContext context)
        {
            //Creating Text Message
            string name = "you";
            Identity User;
            if (context.UserData.TryGetValue(StaticUtils.UserIdentity, out User))
                name = User.GivenName;
            string msg = "Welcome to the "+getCurrentModule(context)+" module help bot.  ";
            //Creating Attachment (Hero Card)
            var hero = new HeroCard
            {
                Title = msg,
              
                Text = "You can ask me general questions about "+ getCurrentModule(context) + "  e.g submitting assignments or the content and focus of weekly lectures or labs. You can come back to this menu at any time by typing \"Hi\". Type \"Help\" for general support.",
                
                
            };

            //Attaching text and attachment to the message
            var message = context.MakeMessage() as IMessageActivity;
            message.Speak = msg;
            message.Attachments = new List<Attachment>() { hero.ToAttachment() };

            await context.PostAsync(message);
            context.Wait(MessageReceived);
        }

        
        private async Task SendHelpMessageAsync(IDialogContext context)
        {
            AdaptiveCard card = new AdaptiveCard();

            card.Body.Add(new TextBlock()
            {
                Text = "Help",
                Weight = TextWeight.Bolder,
                IsSubtle = false,
                Wrap = true,
                Size = TextSize.Normal
            });




            card.Body.Add(new TextBlock()
            {
                
                Text = "You can ask me questions about " + getCurrentModule(context) + " . Check typical questions you can ask about your module  .\n\n>",
                IsSubtle = false,
                Wrap = true,
                Size = TextSize.Normal
            });

            card.Body.Add(new TextBlock()
            {
                Text = "lecturer's information \n> * who is my  lecturer for " + getCurrentModule(context) + " ? \n> * What is the email of my lecturer ?\n> * Where is my lecturer’s office ?\n\n",
                IsSubtle = false,
                Wrap = true,
                Size = TextSize.Normal,


            });
            card.Body.Add(new TextBlock()
            {

                Text = "Lecture and Lab information\n> * When is my lecture/lab on week 1 ?\n> * Where is my lecture \n>* When does lecture/lab start ?",
                IsSubtle = false,
                Wrap = true,
                Size = TextSize.Normal,
                Separation = SeparationStyle.Strong
            });
            card.Body.Add(new TextBlock()
            {


                Text = " General module's information\n> * What is the focus of the module ?\n> * Are there any exams for the module ?\n> * How many pieces of coursework are there for the module ?\n> * What looks like the exam for the module ?",
                IsSubtle = false,
                Wrap = true,
                Size = TextSize.Normal,
                Separation = SeparationStyle.Strong
            });
            card.Body.Add(new TextBlock()
            {


                Text = "Assignment's Information\n> * What do we have to do for coursework 1 ?\n> * When is the coursework 2 due ?\n> * How do we submit the coursework 2 ? ",
                IsSubtle = false,
                Wrap = true,
                Size = TextSize.Normal,
                Separation = SeparationStyle.Strong
            });
            card.Body.Add(new TextBlock()
            {


                Text = "Schedule \n> * What is the schedule for the week 1 ",
                IsSubtle = false,
                Wrap = true,
                Size = TextSize.Normal,
                Separation = SeparationStyle.Strong
            });

            card.Body.Add(new TextBlock()
            {


                Text = "Privacy and Terms ",
                IsSubtle = false,
                Wrap = true,
                Size = TextSize.Normal,
                Separation = SeparationStyle.Strong
            });
            card.Body.Add(new TextBlock()
            {


                Text = "By using this service you agree to the Microsoft Services Agreement at [https://www.microsoft.com/en-us/servicesagreement/](https://www.microsoft.com/en-us/servicesagreement/).\n\n Please read the Microsoft Privacy Statement at [https://go.microsoft.com/fwlink/](https://go.microsoft.com/fwlink/?LinkId=521839) to learn about Microsoft's commitment to privacy",

                IsSubtle = false,
                Wrap = true,
                Size = TextSize.Normal

            });
            Attachment attachment = new Attachment()
            {
                ContentType = AdaptiveCard.ContentType,
                Content = card
            };

            var reply = context.MakeMessage();
            reply.Attachments.Add(attachment);

            await context.PostAsync(reply);
            context.Wait(MessageReceived);
        }


        /// <summary>
        /// Connect the bot to database
        /// </summary>
        /// <param name="builder"></param>
        public void loginSQL(System.Data.SqlClient.SqlConnectionStringBuilder builder)
        {
            builder.DataSource = "scis-botservice.database.windows.net"; //databaseulster.database.windows.net"";
            builder.UserID = "srv_botservice_dbadmin";//Etienne.dupuis "srv_botservice_dbadmin";
            builder.Password = "SC3S_bot6";// Ulsterdatabase1"SC3S_bot6";
            builder.InitialCatalog = "db_SCIS_BotService";// Database_Ulster"db_SCIS_BotService";
        }
       







        //----------------------------------INTENTS------------------------------------

        [LuisIntent("None")]
        
        public async Task None(IDialogContext context, LuisResult result)
        {
            var reply = context.MakeMessage();
            reply.Speak = reply.Text = "I'm sorry. I didn't understand you. Type **\"Help\"** for more help";
            await context.PostAsync(reply);
            context.Wait(MessageReceived);
        }

        [LuisIntent("Greetings")]
        public async Task Greetings(IDialogContext context, LuisResult result)
        {
            await this.SendWelcomeMessageAsync(context);
        }

        [LuisIntent("Help")]
        public async Task Help(IDialogContext context, LuisResult result)
        {
            await this.SendHelpMessageAsync(context);
        }

        /// <summary>
        /// The LecturerName intents is trigerred 
        /// </summary>
        [LuisIntent("LecturerName")]
        public async Task LecturerName(IDialogContext context, LuisResult result)
        {
            //Store data about current intent
            CurrentIntend = "LecturerName";

            //Launch the search for the answer
            await FindAnswer(context, CurrentIntend, new List<string> { ModuleID }, new List<string> { getCurrentModule(context) });
            

        }

        /// <summary>
        /// The LecturerEmail intents is trigerred 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        [LuisIntent("LecturerEmail")]
        public async Task LecturerEmail(IDialogContext context, LuisResult result)
        {
            //Store data about current intent
            CurrentIntend = "LecturerEmail";


            //Launch the search for the answer
            await FindAnswer(context, CurrentIntend, new List<string> { ModuleID }, new List<string> { getCurrentModule(context) });


        }

        /// <summary>
        /// The LecturerPhoneNumber intents is trigerred 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        [LuisIntent("LecturerPhoneNumber")]
        public async Task LecturerPhoneNumber(IDialogContext context, LuisResult result)
        {
            //Store data about current intent
            CurrentIntend = "LecturerPhoneNumber";


            //Launch the search for the answer
            await FindAnswer(context, CurrentIntend, new List<string> { ModuleID }, new List<string> { getCurrentModule(context) });


        }

        /// <summary>
        /// The LecturerOffice intents is trigerred 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        [LuisIntent("LecturerOffice")]
        public async Task LecturerOffice(IDialogContext context, LuisResult result)
        {
            //Store data about current intent
            CurrentIntend = "LecturerOffice";


            //Launch the search for the answer
            await FindAnswer(context, CurrentIntend, new List<string> { ModuleID }, new List<string> { getCurrentModule(context) });

        }

        /// <summary>
        /// The LectureTime intents is trigerred 
        /// </summary>
        [LuisIntent("LectureTime")]
        public async Task LectureTime(IDialogContext context, LuisResult result)
        {
            //Store data about current intent
            CurrentIntend = "LectureTime";

            EntityRecommendation MyLectureType;
            if (result.TryFindEntity(LectureType, out MyLectureType))
            {
                isLectureType = true;
        
        EntityRecommendation MyWeekNumber;
                if (result.TryFindEntity(Week, out MyWeekNumber))
                {
                    
                   
                    //Launch the search for the answer
                    await FindAnswer(context, CurrentIntend, new List<string> { ModuleID, LectureType, Week }, new List<string> { getCurrentModule(context), CanonicalEntityToString(MyLectureType), StaticUtils.WeekNumber[CanonicalEntityToString(MyWeekNumber)] });
                }
                else
                {


                    //Entities (extract the canonnical form from the luis answer Please refer to luis entities list for more informnation)
                    CurrentLectureType = CanonicalEntityToString(MyLectureType);

                    string entityFamiliarName = "Week";
                    string entityRequest = " week number";
                    ////Launch the dialogs asking further information to the user
                    LaunchFurtherInformation(context, new List<string> { Week }, entityFamiliarName, entityRequest);
                   

                }
                
            }
            else
            {
                EntityRecommendation MyWeekNumber;
                if (result.TryFindEntity(Week, out MyWeekNumber))
                {
                    //Launch the search for the answer
                    await FindAnswer(context, CurrentIntend, new List<string> { ModuleID, Week }, new List<string> { getCurrentModule(context), StaticUtils.WeekNumber[CanonicalEntityToString(MyWeekNumber)] });
                }
                else
                {
                    //Launch the dialogs asking further information to the user
                    string entityFamiliarName = "Week";
                    string entityRequest = " week number";
                    LaunchFurtherInformation(context, new List<string> { Week }, entityFamiliarName, entityRequest);
                }
            }

        }

        /// <summary>
        /// The LectureLocation intents is trigerred 
        /// </summary>
        [LuisIntent("LectureLocation")]
        public async Task LectureLocation(IDialogContext context, LuisResult result)
        {
            //Store data about current intent

            CurrentIntend = "LectureLocation";

            EntityRecommendation MyLectureType;
            if (result.TryFindEntity(LectureType, out MyLectureType))
            {
                //Launch the search for the answer
                await FindAnswer(context, CurrentIntend, new List<string> { ModuleID, LectureType }, new List<string> { getCurrentModule(context), CanonicalEntityToString(MyLectureType) });

            }
            else
            {
                //Launch the dialogs asking further information to the user
                string entityFamiliarName = "lecture type";
                string entityRequest = " lecture type: Lab or Lecture ";
                LaunchFurtherInformation(context, new List<string> { LectureType }, entityFamiliarName, entityRequest);

            }

        }



        /// <summary>
        /// The LectureBegining intents is trigerred 
        /// </summary>
        [LuisIntent("LectureBegining")]
        public async Task LectureBegining(IDialogContext context, LuisResult result)
        {
            //Store data about current intent
            CurrentIntend = "LectureBegining";

            EntityRecommendation MyWeekNumber;

            if (result.TryFindEntity(Week, out MyWeekNumber))
            {
                //Launch the search for the answer
                await FindAnswer(context, CurrentIntend, new List<string> { ModuleID, Week }, new List<string> { getCurrentModule(context), StaticUtils.WeekNumber[CanonicalEntityToString(MyWeekNumber)] });
            }
            else
            {
                //Launch the dialogs asking further information to the user
                string entityFamiliarName = "Week";
                string entityRequest = " week number";
                LaunchFurtherInformation(context, new List<string> { Week }, entityFamiliarName, entityRequest);
            }


        }

        /// <summary>
        /// The ModuleOverview intents is trigerred 
        /// </summary>
        [LuisIntent("ModuleOverview")]
        public async Task ModuleOverview(IDialogContext context, LuisResult result)
        {
            //Store data about current intent
            CurrentIntend = "ModuleOverview";



            //Launch the search for the answer
            await FindAnswer(context, CurrentIntend, new List<string> { ModuleID }, new List<string> { getCurrentModule(context) });


        }

        /// <summary>
        /// The Schedule intents is trigerred 
        /// </summary>
        [LuisIntent("Schedule")]
        public async Task Schedule(IDialogContext context, LuisResult result)
        {
            //Store data about current intent
            CurrentIntend = "Schedule";

            EntityRecommendation MyLectureType;
            if (result.TryFindEntity(LectureType, out MyLectureType))
            {
                isLectureType = true;

                EntityRecommendation MyWeekNumber;
                if (result.TryFindEntity(Week, out MyWeekNumber))
                {

                    //Launch the search for the answer
                    await FindAnswer(context, CurrentIntend, new List<string> { ModuleID, LectureType, Week }, new List<string> { getCurrentModule(context), CanonicalEntityToString(MyLectureType), StaticUtils.WeekNumber[CanonicalEntityToString(MyWeekNumber)] });
                }
                else
                {
                    //Launch the dialogs asking further information to the user
                    CurrentLectureType = CanonicalEntityToString(MyLectureType);
                    string entityFamiliarName = "a week number ?";
                    string entityRequest = " week number ";
                    LaunchFurtherInformation(context, new List<string> { Week }, entityFamiliarName, entityRequest);


                }

            }
            else
            {
                EntityRecommendation MyWeekNumber;
                if (result.TryFindEntity(Week, out MyWeekNumber))
                {
                    //Launch the search for the answer
                    await FindAnswer(context, CurrentIntend, new List<string> { ModuleID, Week }, new List<string> { getCurrentModule(context), StaticUtils.WeekNumber[CanonicalEntityToString(MyWeekNumber)] });
                }
                else
                {
                    //Launch the dialogs asking further information to the user
                    string entityFamiliarName = "Week";
                    string entityRequest = " week number ";
                    LaunchFurtherInformation(context, new List<string> { Week }, entityFamiliarName, entityRequest);
                }
            }
        }

        /// <summary>
        /// The RessourceList intents is trigerred 
        /// </summary>
        [LuisIntent("ResourceList")]
        public async Task RessourceList(IDialogContext context, LuisResult result)
        {
            //Store data about current intent
            CurrentIntend = "ResourceList";

            EntityRecommendation MyRessourceType;
            if (result.TryFindEntity(RessourceType, out MyRessourceType))
            {
                CurrentResourceType = CanonicalEntityToString(MyRessourceType);

                //Launch the search for the answer

                await FindAnswer(context, CurrentIntend, new List<string> { ModuleID, RessourceType }, new List<string> { getCurrentModule(context), CanonicalEntityToString(MyRessourceType) });

                CurrentResourceType = "";
            }
            else
            {
                await FindAnswer(
                    context,
                    "ResourceList",
                    new List<string> { ModuleID },
                    new List<string> { getCurrentModule(context) }
                    );

            }

        }

        /// <summary>
        /// The ModuleAssessmentOverview intents is trigerred 
        /// </summary>
        [LuisIntent("ModuleAssessmentOverview")]
        public async Task ModuleAssessmentOverview(IDialogContext context, LuisResult result)
        {
            //Store data about current intent
            CurrentIntend = "ModuleAssessmentOverview";

            await FindAnswer(context, CurrentIntend, new List<string> { ModuleID }, new List<string> { getCurrentModule(context) });


        }

        /// <summary>
        /// The ModuleAssessmentNumber intents is trigerred 
        /// </summary>
        [LuisIntent("ModuleAssessmentNumber")]
        public async Task ModuleAssessmentNumber(IDialogContext context, LuisResult result)
        {
            //Store data about current intent
            CurrentIntend = "ModuleAssessmentNumber";
            



            //Launch the search for the answer
            await FindAnswer(context, CurrentIntend, new List<string> { ModuleID }, new List<string> { getCurrentModule(context) });


        }

        /// <summary>
        /// The DueDate intents is trigerred 
        /// </summary>
        [LuisIntent("DueDate")]
        public async Task DueDate(IDialogContext context, LuisResult result)
        {
            //Store data about current intent
            CurrentIntend = "DueDate";

            EntityRecommendation MyAssessmentNumber;
            if (result.TryFindEntity(AssessmentNumber, out MyAssessmentNumber))
            {


                //Launch the search for the answer

                await FindAnswer(context, CurrentIntend, new List<string> { ModuleID, AssessmentNumber }, new List<string> { getCurrentModule(context), CanonicalEntityToString(MyAssessmentNumber) });



            }
            else
            {

                string entityFamiliarName = "Assessment";
                string entityRequest = "assessment name or number";
                LaunchFurtherInformation(context, new List<string> { AssessmentNumber }, entityFamiliarName, entityRequest);

            }

        }

        /// <summary>
        /// The AssessmentWeighing intents is trigerred 
        /// </summary>
        [LuisIntent("AssessmentWeighing")]
        public async Task AssessmentWeighing(IDialogContext context, LuisResult result)
        {
            //Store data about current intent

            CurrentIntend = "AssessmentWeighing";

            EntityRecommendation MyAssessmentNumber;
            if (result.TryFindEntity(AssessmentNumber, out MyAssessmentNumber))
            {


                //Launch the search for the answer

                await FindAnswer(context, CurrentIntend, new List<string> { ModuleID, AssessmentNumber }, new List<string> { getCurrentModule(context), CanonicalEntityToString(MyAssessmentNumber) });



            }
            else
            {

                string entityFamiliarName = "Assessment";
                string entityRequest = "assessment name or number";
                LaunchFurtherInformation(context, new List<string> { AssessmentNumber }, entityFamiliarName, entityRequest);

            }

        }

        /// <summary>
        /// The SubmissionProcess intents is trigerred 
        /// </summary>
        [LuisIntent("SubmissionProcess")]
        public async Task SubmissionProcess(IDialogContext context, LuisResult result)
        {
            //Store data about current intent
            CurrentIntend = "SubmissionProcess";

            EntityRecommendation MyAssessmentNumber;
            if (result.TryFindEntity(AssessmentNumber, out MyAssessmentNumber))
            {

                //Launch the search for the answer

                await FindAnswer(context, CurrentIntend, new List<string> { ModuleID, AssessmentNumber }, new List<string> { getCurrentModule(context), CanonicalEntityToString(MyAssessmentNumber) });

            }
            else
            {
                string entityFamiliarName = "Assessment";
                string entityRequest = "assessment name or number";
                LaunchFurtherInformation(context, new List<string> { AssessmentNumber }, entityFamiliarName, entityRequest);


            }
        }

        /// <summary>
        /// The Objectives intents is trigerred 
        /// </summary>
        [LuisIntent("Objectives")]
        public async Task Objectives(IDialogContext context, LuisResult result)
        {
            //Store data about current intent
            CurrentIntend = "Objectives";

            EntityRecommendation MyAssessmentNumber;
            if (result.TryFindEntity(AssessmentNumber, out MyAssessmentNumber))
            {

                //Launch the search for the answer

                await FindAnswer(context, CurrentIntend, new List<string> { ModuleID, AssessmentNumber }, new List<string> { getCurrentModule(context), CanonicalEntityToString(MyAssessmentNumber) });

            }
            else
            {
                string entityFamiliarName = "Assessment";
                string entityRequest = "assessment name or number";
                LaunchFurtherInformation(context, new List<string> { AssessmentNumber }, entityFamiliarName, entityRequest);
                

               
            }
        }

        /// <summary>
        /// The Weighing intents is trigerred 
        /// </summary>
        [LuisIntent("Weighting")]
        public async Task Weighting(IDialogContext context, LuisResult result)
        {
            //Store data about current intent
            CurrentIntend = "Weighting";

            //Launch the search for the answer
            await FindAnswer(context, CurrentIntend, new List<string> { ModuleID }, new List<string> { getCurrentModule(context) });


        }

        /// <summary>
        /// The TeamAssignmentOverview intents is trigerred 
        /// </summary>
        [LuisIntent("TeamAssignmentOverview")]
        public async Task TeamAssignmentOverview(IDialogContext context, LuisResult result)
        {
            CurrentIntend = "TeamAssignmentOverview";
            //Launch the search for the answer
            await FindAnswer(context, CurrentIntend, new List<string> { ModuleID }, new List<string> { getCurrentModule(context) });

        }

        /// <summary>
        /// The ExamSchedule intents is trigerred 
        /// </summary>
        [LuisIntent("ExamSchedule")]
        public async Task ExamSchedule(IDialogContext context, LuisResult result)
        {
            //Store data about current intent

            CurrentIntend = "ExamSchedule";

            //Launch the search for the answer
            await FindAnswer(context, CurrentIntend, new List<string> { ModuleID }, new List<string> { getCurrentModule(context) });

        }

        /// <summary>
        /// The ExamFormat intents is trigerred 
        /// </summary>
        [LuisIntent("ExamFormat")]
        public async Task ExamFormat(IDialogContext context, LuisResult result)
        {
            //Store data about current intent
            CurrentIntend = "ExamFormat";


            //Launch the search for the answer
            await FindAnswer(context, CurrentIntend, new List<string> { ModuleID }, new List<string> { getCurrentModule(context) });


        }

        /// <summary>
        /// The FeedbackDate intents is trigerred 
        /// </summary>
        [LuisIntent("FeedbackDate")]
        public async Task FeedbackDate(IDialogContext context, LuisResult result)
        {
            //Store data about current intent
            CurrentIntend = "FeedbackDate";

            EntityRecommendation MyAssessmentNumber;
            if (result.TryFindEntity(AssessmentNumber, out MyAssessmentNumber))
            {
                //Launch the search for the answer
                await FindAnswer(context, CurrentIntend, new List<string> { ModuleID, AssessmentNumber }, new List<string> { getCurrentModule(context), CanonicalEntityToString(MyAssessmentNumber) });


                


            }
            else
            {
                string entityFamiliarName = "Assessment";
                string entityRequest = "assessment name or number";
                LaunchFurtherInformation(context, new List<string> { AssessmentNumber }, entityFamiliarName, entityRequest);
               


            }
        }

        //----------------------------------METHODS------------------------------------

        /// <summary>
        /// Find the answer to the current intents of the conversationnal data, all entities have to be in the conversation data and their id in the ListEntityID
        /// You also  must have specify a request name in the conversationnal data 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="Intent">name of intend</param>
        /// <param name="EntityID">entity names list</param>
        /// <param name="EntityValue">entity name values list</param>
        /// <returns></returns>

        private async Task FindAnswer(IDialogContext context, string Intent, List<string> EntityID, List<string> EntityValue)

        {

            Boolean isLectureType=false;
            Boolean isWeeknumber= false;
            //Selecting the right SQL query for the intents and entities combination 
            string queryName = Intent + "Query";
            string answerName = Intent + "Answer";
            if (EntityID.Contains(ModuleID))
            {
                queryName += "_MODID"; answerName += "_MODID";
            }
            if (EntityID.Contains(LecturerNameEntity))
            {
                queryName += "_LECTNM"; answerName += "_LECTNM";
            }
            if (EntityID.Contains(RessourceType))
            {
                queryName += "_RESSTYPE"; answerName += "_RESSTYPE";
            }
            if (EntityID.Contains(AssessmentNumber))
            {
                queryName += "_ASSNUMB"; answerName += "_ASSNUMB";
                
            }
            if (EntityID.Contains(LectureType))
            {
                queryName += "_LECTTYPE"; answerName += "_LECTTYPE";
                isLectureType = true;
            }
            if (EntityID.Contains(Week))
            {
                queryName += "_WNUMB"; answerName += "_WNUMB";
                isWeeknumber = true;
            }
            string tsql = StaticUtils.QueryIndex[queryName];

            //Writing detected entities as param for the sql query
            List<SqlParameter> ListParam = new List<SqlParameter>();
            List<string> EntityResList = new List<string>();

            if (EntityID.Count != EntityValue.Count)
                throw new Exception("Wrong size list");

            for (int i = 0; i < EntityID.Count; i++)
            {
                ListParam.Add(MakeParam(EntityValue[i], EntityID[i]));
                EntityResList.Add(EntityValue[0]);
            }
            
            //Executing the query
            List<List<string>> queryresult = ExecSQLParam(tsql, ListParam);

            //Analysing the result and creating an answer for the user
            var reply = context.MakeMessage();
            if (queryresult != null && queryresult.Count > 0 && queryresult[0].Count > 0)
            {
                if (CurrentIntend == "ResourceList")
                {
                    string reponseResource = "For module " + EntityResList[0] + " " + "resources " + CurrentResourceType + " available are:\n\n";
                   
                    for (int i = 0; i < queryresult.Count; i++)
                    {
                        reponseResource += queryresult[i][0] + " : " + queryresult[i][1] + "\n\n";
                    }
                    reply.Speak = reply.Text = reponseResource;

                }
                else
                if (CurrentIntend == "LectureBegining")
                {
                    string reponseLectureBeginning = "Teaching for " + EntityResList[0] + " starts:\n\n"; 
                    for (int i = 0; i < queryresult.Count; i++)
                    {
                        reponseLectureBeginning += " on " + queryresult[i][0] + " at " + queryresult[i][1] + " in " + queryresult[i][2] + " \n\n";

                    }
                    reply.Speak = reply.Text; 
                    reply.Text =  reponseLectureBeginning;
                }
                else
                if (CurrentIntend == "LectureTime")
                {
                    string reponse1 =" The Courses for "+ EntityResList[0] + " on week "+ EntityValue[1] + " :\n\n";
                    for (int i = 0; i < queryresult.Count; i++)
                    {
                        reponse1 += " on " + queryresult[i][0] + " at " + queryresult[i][1] + " in semester " + queryresult[i][2] + " \n\n";
                    }
                    reply.Speak = reply.Text; 
                    reply.Text =  reponse1;
                }
                else

                if (CurrentIntend == "Schedule")
                {
                    string reponse1 = "";
                    
                    if (isLectureType == true)
                    {
                        if (isWeeknumber == true)
                        {
                            reponse1 = " The schedule for " + EntityResList[0] + " in " + EntityValue[1] + " on week " + EntityValue[2] + " is :\n\n";
                            for (int i = 0; i < queryresult.Count; i++)
                            {
                                reponse1 += "On " + queryresult[i][0] + " at " + queryresult[i][1] + " in room " + queryresult[i][3] + ":" + queryresult[i][2] + " \n\n";
                            }
                            reply.Speak = reply.Text; 
                    reply.Text =  reponse1;

                        }
                        else
                        {
                            reponse1 = " The schedule for " + EntityResList[0] + " in " + EntityValue[1] +  " is :\n\n";
                            for (int i = 0; i < queryresult.Count; i++)
                            {
                                reponse1 += "On " + queryresult[i][0] + " at " + queryresult[i][1] + " in room " + queryresult[i][3] + ":" + queryresult[i][2] + " \n\n";
                            }
                            reply.Speak = reply.Text; 
                    reply.Text =  reponse1;
                        }
                            
                    }

                    else {

                        if (isWeeknumber == true)
                        {
                            reponse1 = " The schedule for " + EntityResList[0] + " on week " + EntityValue[1] + " is :\n\n";
                            for (int i = 0; i < queryresult.Count; i++)
                            {
                                reponse1 += "On " + queryresult[i][0] + " at " + queryresult[i][1] + " in room " + queryresult[i][3] + ":" + queryresult[i][2] + " \n\n";
                            }
                            reply.Speak = reply.Text; 
                    reply.Text =  reponse1;

                        }
                        else
                        {
                            reponse1 = " The schedule for " + EntityResList[0] +" is :\n\n";
                            for (int i = 0; i < queryresult.Count; i++)
                            {
                                reponse1 += "On " + queryresult[i][0] + " at " + queryresult[i][1] + " in room " + queryresult[i][3] + ":" + queryresult[i][2] + " \n\n";
                            }
                            reply.Speak = reply.Text; 
                             reply.Text =  reponse1;
                        }
                    }
                     
                }

                else
                {
                    StaticUtils.Answer answer = StaticUtils.AnswerIndex[answerName];
                    if (answer.fieldsNumber == 1)
                    {
                        string reponse = String.Format(answer.value, EntityResList[0], queryresult[0][0]);
                        reply.Speak = reply.Text = reponse;
                    }
                    else
                  if (answer.fieldsNumber == 2)
                    {
                        string reponse1 = String.Format(answer.value, EntityResList[0], queryresult[0][0], queryresult[0][1]);
                        // string reponse4 = "**Assignment 1 guidelines**\n\nTo become a successful game designer, you must critically analyze existing games at a system level.You need to learn to separate the game systems from the user experience and also view and understand a game from the perspective of the target end user. For assignment 1 you are required to carry out a game analysis as part of a team and present the results / outcomes as a video created using Office Mix. You must document the assignment planning and implementation process on Slack / Trello.There should be clear evidence of planning, deliverables and task allocation over the period of the assignment.The game you must analyze is Candy Crush Saga and you must use a similar approach as taken in the Extra Credits Puzzle Game video. You should cover the following material in the analysis.\n\n**Game introduction and overview** \n\n What are the basic mechanics of Candy Crush Saga ?\n\n• How does the levelling system work?\n\n• How many points does it take to beat the introductory levels(1 - 6) ?\n\n• What is the relationship between moves and score and levelling up ?\n\n• How many points do you get for matching 3 and 4 candies on each level ?\n\n• What is the losing condition ?\n\n• Discuss the main differences between Bejeweled 2 and Candy Crush Saga ?\n\n• Players perspective i.e.how do they experience the game ?\n\n• Add any other observations you feel worthy of note!\n\n**Outputs**\n\n• 8 - 10 minute video created in Office Mix exported in MP4 Format\n\n• Video should be of high quality and with clear audio\n\n• Video should have a title screen with the project title, your team number, team names and student numbers \n\n• Video should use voice annotations with captions, titles and call - outs as appropriate \n\n• Each team member should contribute and be included in the audio track\n\n Implementation process as a team should be catalogued on Slack and Trello\n\n";

                        reply.Speak = reply.Text = reponse1;
                    }
                    else
                  if (answer.fieldsNumber == 3)
                    {
                        string reponse2 = String.Format(answer.value, EntityResList[0], queryresult[0][0], queryresult[0][1], queryresult[0][2]);
                        
                        reply.Speak = reply.Text = reponse2;
                    }
                    else
                        reply.Speak = reply.Text = "Internal Problem (unable to get answer's number of fields)";
                }
            }
            else
            {
                reply.Speak = reply.Text = "No results found, Sorry but the desired information is not in my knowledge base";
            }
            //Sending answer to user
            await context.PostAsync(reply);

            //resume dialog
            context.Wait(MessageReceived);
        }
        private IMessageActivity Terms(IDialogContext context)
        {
            AdaptiveCard card = new AdaptiveCard();

            card.Body.Add(new TextBlock()
            {
                Text = "Privacy and Terms of Use",
                IsSubtle = false,
                Wrap = true,
                Size = TextSize.Normal,
                Weight = TextWeight.Bolder
            });



            card.Body.Add(new TextBlock()
            {
                Text = "Martha is enabled by Microsoft Bot Framework. The Microsoft Bot Framework is a set of web-services that enable intelligent services and connections using conversation channels you authorize. As a service provider, Microsoft will transmit content you provide to our bot/service in order to enable the service.",

                IsSubtle = false,
                Wrap = true,
                Size = TextSize.Normal

            });


            card.Body.Add(new TextBlock()
            {
                Text = "For more information about Microsoft privacy policies please see their privacy statement here:",

                IsSubtle = false,
                Wrap = true,
                Size = TextSize.Normal

            });


            card.Body.Add(new TextBlock()
            {
                Text = "**[https://go.microsoft.com/fwlink/](https://go.microsoft.com/fwlink/?LinkId=521839)**",

                IsSubtle = false,
                Wrap = true,
                Size = TextSize.Normal

            });


            card.Body.Add(new TextBlock()
            {
                Text = "In addition, your interactions with this bot/service are also subject to the conversational channel's applicable terms of use, privacy and data collection policies.",

                IsSubtle = false,
                Wrap = true,
                Size = TextSize.Normal

            });

            card.Body.Add(new TextBlock()
            {
                Text = "By using this service you agree to the University's IT and data protection policies.",

                IsSubtle = false,
                Wrap = true,
                Size = TextSize.Normal

            });

            card.Body.Add(new TextBlock()
            {
                Text = "**Ulster University Data Protection Policies**",

                IsSubtle = false,
                Wrap = true,
                Size = TextSize.Normal

            });

            card.Body.Add(new TextBlock()
            {
                Text = "**[https://www.ulster.ac.uk/__data/assets/pdf_file/0011/120998/Data-Protection-Policy.pdf](https://www.ulster.ac.uk/__data/assets/pdf_file/0011/120998/Data-Protection-Policy.pdf)**",

                IsSubtle = false,
                Wrap = true,
                Size = TextSize.Normal

            });


            card.Body.Add(new TextBlock()
            {
                Text = "**Ulster University IT Policies**",

                IsSubtle = false,
                Wrap = true,
                Size = TextSize.Normal

            });

            card.Body.Add(new TextBlock()
            {
                Text = "**[https://www.ulster.ac.uk/isd/it-policies](https://www.ulster.ac.uk/isd/it-policies)**",

                IsSubtle = false,
                Wrap = true,
                Size = TextSize.Normal

            });



            card.Body.Add(new TextBlock()
            {
                Text = "To report abuse when using a bot that uses the Microsoft Bot Framework to Microsoft, please visit the Microsoft Bot Framework website at **[https://www.botframework.com/](https://www.botframework.com/)** and use the “Report Abuse” link in the menu to contact Microsoft.",

                IsSubtle = false,
                Wrap = true,
                Size = TextSize.Normal

            });





            card.Body.Add(new TextBlock()
            {
                Text = "By using this service you agree to the Microsoft Services Agreement.",

                IsSubtle = false,
                Wrap = true,
                Size = TextSize.Normal

            });

            card.Body.Add(new TextBlock()
            {
                Text = "**[https://www.microsoft.com/en-us/servicesagreement/](https://www.microsoft.com/en-us/servicesagreement/)**",

                IsSubtle = false,
                Wrap = true,
                Size = TextSize.Normal

            });

            card.Body.Add(new TextBlock()
            {
                Text = "**About the project**",

                IsSubtle = false,
                Wrap = true,
                Size = TextSize.Normal

            });

            card.Body.Add(new TextBlock()
            {
                Text = "This is a pilot project currently in beta and under development.",

                IsSubtle = false,
                Wrap = true,
                Size = TextSize.Normal

            });

            card.Body.Add(new TextBlock()
            {
                Text = "The information provided is for general guidance only. For up to date information on the University's policies and regulations please go here.",

                IsSubtle = false,
                Wrap = true,
                Size = TextSize.Normal

            });


            card.Body.Add(new TextBlock()
            {
                Text = "**Procedures & regulations**",

                IsSubtle = false,
                Wrap = true,
                Size = TextSize.Normal

            });

            card.Body.Add(new TextBlock()
            {
                Text = "**[http://www.ulster.ac.uk/qaguide/procedures-regulations/](http://www.ulster.ac.uk/qaguide/procedures-regulations/)**",

                IsSubtle = false,
                Wrap = true,
                Size = TextSize.Normal

            });


            Microsoft.Bot.Connector.Attachment attachment = new Microsoft.Bot.Connector.Attachment()
            {
                ContentType = AdaptiveCard.ContentType,
                Content = card
            };

            var reply = context.MakeMessage();
            reply.Attachments.Add(attachment);

            return reply;
            //  var message = context.MakeMessage() as IMessageActivity;
            // message.Speak = message.Text = "By using this service you agree to the Microsoft Services Agreement at **https://www.microsoft.com/en-us/servicesagreement/** . Please read the Microsoft Privacy Statement at **https://go.microsoft.com/fwlink/?LinkId=521839** to learn about Microsoft's commitment to privacy";

        }
        private void LaunchFurtherInformation(IDialogContext context, List<string> EntityID, string entityFamiliarName, string entityRequest)
        {
            //Call the further information dialog
            context.Call(new FurtherInformationDialog(EntityID, entityFamiliarName, entityRequest), this.FurtherInformationResume);
        }
        /// <summary>
        /// Resume after the FurtherInformation Dialog
        /// </summary>
        private List<List<string>> ExecSQLParam(string tsql, List<SqlParameter> ListParam)
        {
            var builder = new SqlConnectionStringBuilder();
            List<List<string>> queryresult = new List<List<string>>();

            loginSQL(builder);
            using (var connection = new SqlConnection(builder.ConnectionString))
            {
                connection.Open();
                using (var command = new SqlCommand(tsql, connection))
                {
                    foreach (var parameters in ListParam)
                    {
                        command.Parameters.Add(parameters);
                    }
                    using (SqlDataReader reader = command.ExecuteReader())
                    {

                        while (reader.Read())
                        {

                            List<string> row = new List<string>();
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                object value = reader.GetValue(i);
                                if (value is int)
                                {
                                    string ConvertedValue = "" + value;
                                    row.Add(ConvertedValue);
                                }

                                if (value is string)
                                {
                                    row.Add(reader.GetString(i));
                                }
                            }
                            queryresult.Add(row);
                        }
                    }
                }
            }
            return queryresult;
        }
        private static void ExtractCanonicalEntity(IDialogContext context, EntityRecommendation MyLectureType, string entityType)
        {
            Newtonsoft.Json.Linq.JArray resultFromIntents = MyLectureType.Resolution["values"] as Newtonsoft.Json.Linq.JArray;
            context.ConversationData.SetValue(entityType, resultFromIntents.Value<string>(0));
        }

        private static string CanonicalEntityToString(EntityRecommendation MyEntity)
        {
            Newtonsoft.Json.Linq.JArray resultFromIntents = MyEntity.Resolution["values"] as Newtonsoft.Json.Linq.JArray;
            return resultFromIntents.Value<string>(0);
        }
        private static SqlParameter MakeParam(string Entity, string paramTag)
        {
            SqlParameter param = new SqlParameter();
            param.ParameterName = paramTag;
            param.Value = Entity;
            return param;
        }
        private async Task FurtherInformationResume(IDialogContext context, IAwaitable<string> result)
        {
            var msg = await result;
            string EntityID = null;
            string EntityValue = null;
            context.ConversationData.TryGetValue("name entity", out EntityID);
            context.ConversationData.TryGetValue("value entity", out EntityValue);
            if (msg == StaticUtils.TooManyAttempts)//case user try too many times to specify a module id
            {
                var reply = context.MakeMessage();
                reply.Speak = reply.Text = $"Sorry, I did not understand. Please try another question";
                await context.PostAsync(reply);
                context.Wait(MessageReceived);

            }
            else
            {
                if (EntityValue == "general schedule")
                {
                    if (isLectureType == true)
                    {

                        await FindAnswer(context, CurrentIntend, new List<string> { ModuleID, LectureType }, new List<string> { getCurrentModule(context),  CurrentLectureType });
                        isLectureType = false;
                        CurrentLectureType = "";
                    }
                    else
                    {
                        await FindAnswer(context, CurrentIntend, new List<string> { ModuleID }, new List<string> { getCurrentModule(context) });
                        isLectureType = false;
                        CurrentLectureType = "";
                    }
                }
                else
                {
                    try
                    {


                        if (isLectureType == true)
                        {

                            await FindAnswer(context, CurrentIntend, new List<string> { ModuleID, EntityID, LectureType }, new List<string> { getCurrentModule(context), EntityValue, CurrentLectureType });
                            isLectureType = false;
                            CurrentLectureType = "";
                        }
                        else
                        {
                            await FindAnswer(context, CurrentIntend, new List<string> { ModuleID, EntityID }, new List<string> { getCurrentModule(context), EntityValue });
                            isLectureType = false;
                            CurrentLectureType = "";
                        }

                    }
                      catch
                         {
                             await context.PostAsync("I'm sorry, I'm having issues understanding you. Let's try again.");
                             await this.SendWelcomeMessageAsync(context);
                             isLectureType = false;
                             CurrentLectureType = "";
                         }
                }



            }

        }
    }