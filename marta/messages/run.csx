#r "Newtonsoft.Json"

#load "LuisDialogBlackboard.csx"
#load "StaticUtils.csx"
using System;
using System.Net;
using System.Threading;
using Newtonsoft.Json;

using Microsoft.Bot.Builder.Azure;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using AdaptiveCards;


public static async Task<object> Run(HttpRequestMessage req, TraceWriter log)
{
    log.Info($"Webhook was triggered!");
 
    // Initialize the azure bot
    using (BotService.Initialize())
    {
        // Deserialize the incoming activity
        string jsonContent = await req.Content.ReadAsStringAsync();
        var activity = JsonConvert.DeserializeObject<Activity>(jsonContent);
    
        // authenticate incoming request and add activity.ServiceUrl to MicrosoftAppCredentials.TrustedHostNames
        // if request is authenticated
        if (!await BotService.Authenticator.TryAuthenticateAsync(req, new [] {activity}, CancellationToken.None))
        {
            return BotAuthenticator.GenerateUnauthorizedResponse(req);
        }
    
        if (activity != null)
        {
            // one of these will have an interface and process it
            switch (activity.GetActivityType())
            {
                case ActivityTypes.Message:
                    //Choose a script in relation to a specific channel
                    if(activity.ChannelId.Equals("webchat"))
                    {   
                        //Cortana script
                       ConnectorClient connector = new ConnectorClient(new System.Uri(activity.ServiceUrl));

                    //

                   // Activity reply = activity.CreateReply("");

                   //print Activity ID of link
                   //reply.Text = activity.Recipient.Id;
                   //await connector.Conversations.ReplyToActivityAsync(reply);

                    

                    //launch conversation
                     await Conversation.SendAsync(activity, () => new LuisDialogBlackboard(activity.Recipient.Id));
                    }
                    else{
                        //All channels without Cortana
                       await Conversation.SendAsync(activity, () => new LuisDialogBlackboard("bs-modulebot_R5L3KvSqJF@doKw8Xk8jYY"));

                    }
                    break;
                case ActivityTypes.ConversationUpdate:
                    //Send a welcome card when a user join the conversation
                    IConversationUpdateActivity iConversationUpdated = activity as IConversationUpdateActivity;
                    if (iConversationUpdated != null)
                    {
                        ConnectorClient connector = new ConnectorClient(new System.Uri(activity.ServiceUrl));
                        foreach (var member in iConversationUpdated.MembersAdded ?? System.Array.Empty<ChannelAccount>())
                        {
                            // if the bot is added, then 
                            if (member.Id == iConversationUpdated.Recipient.Id)
                            {   
                                //Welcome message
                                Activity reply = activity.CreateReply();
                                reply.Attachments = new List<Attachment>();
                             
                             AdaptiveCard card = new AdaptiveCard();
                            // Add image to the card.
                            card.Body.Add(new Image()
                            {
                                Url = "https://pbs.twimg.com/profile_images/723468438681866241/duf2-h3L_400x400.jpg",
                                Size = ImageSize.Medium,
                                Style = ImageStyle.Person,
                                HorizontalAlignment = HorizontalAlignment.Center,
                                AltText = "imgBot" 
                            });
                            // Add text to the card.
                            card.Body.Add(new TextBlock()
                            {
                                Text = "Hello, I am the **\""+ StaticUtils.ModuleKey[activity.Recipient.Id] + " help bot\"** and I am here to help.",
                                Size = TextSize.Normal,
                                Weight = TextWeight.Normal,
                                HorizontalAlignment = HorizontalAlignment.Left,
                                Wrap = true
                            });
                            card.Body.Add(new TextBlock()
                            {
                                Text = "You can ask me about your timetable, coursework, exam format and much more.",
                                Size = TextSize.Normal,
                                Weight = TextWeight.Normal,
                                HorizontalAlignment = HorizontalAlignment.Left,
                                Wrap = true
                            });
                            card.Body.Add(new TextBlock()
                            {
                                Text = "Type **\"Hi\"** if you would like to chat or **\"Help\"** for general support.",
                                Size = TextSize.Normal,
                                Weight = TextWeight.Normal,
                                HorizontalAlignment = HorizontalAlignment.Left,
                                Wrap = true
                            });
                            card.Body.Add(new TextBlock()
                            {
                                Text = "By using this service you agree to the Microsoft Services Agreement.",
                                Size = TextSize.Normal,
                                Weight = TextWeight.Normal,
                                HorizontalAlignment = HorizontalAlignment.Left,
                                Wrap = true
                            });
                            card.Body.Add(new TextBlock()
                            {
                                Text = "Type **\"Terms\"** to learn more about our Services Agreement and Privacy Statement.",
                                Size = TextSize.Normal,
                                Weight = TextWeight.Normal,
                                HorizontalAlignment = HorizontalAlignment.Left,
                                Wrap = true
                            });

                                
                                // Create the attachment.
                                Attachment attachment = new Attachment()
                                {
                                    ContentType = AdaptiveCard.ContentType,
                                    Content = card
                                };
                                
                               reply.Attachments.Add(attachment);
    
                               await connector.Conversations.ReplyToActivityAsync(reply);
                            }
                        }
                    }
                    break;
                case ActivityTypes.ContactRelationUpdate:
                case ActivityTypes.Typing:
                case ActivityTypes.DeleteUserData:
                case ActivityTypes.Ping:
                default:
                    log.Error($"Unknown activity type ignored: {activity.GetActivityType()}");
                    break;
            }
        }
        return req.CreateResponse(HttpStatusCode.Accepted);
    }    
}