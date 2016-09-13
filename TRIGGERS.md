# How to create and add your own triggers
This is a little guide on how to create your own triggers. Note: This does not go over the actuall trigger class itself, you need to write that on your own. Look in the SteamChatBot/Triggers folder for examples.

These are the steps you need to take (not necessarily in this order)

**1. Create the trigger class**
This is your main file where all the action takes place. I will not go over this in detail. Look at other triggers in SteamChatBot/Triggers for help.

**2. OPTIONAL: Create the options class**
This is the options class that will go into SteamChatBot/Triggers/Options. It is where the options of your trigger are stored (an example would be a command that the user would enter). This is only optional because you may not need to create a new class for the options. A bunch of options classes already exist. If you are creating your own options file, you must add it to TriggerOptionsBase.cs in SteamChatBot/Triggers/TriggerOptions.
If you need to create your own options, please follow this step and the next.

**2a. OPTIONAL: Create the options window**
To have your own options class you must create your options window. This is a XAML file that is stored in SteamChatBot/Triggers/TriggerOptions/Windows. Look in one of the .xaml.cs files for an example of how to do this.

**2b. Add your trigger to BaseTrigger** 
To finalize everything on the trigger side you will need to add a reference to your trigger to multiple places within the BaseTrigger class. Here is a list of them:

1. [BaseTrigger.ReadTriggers()](https://github.com/Steam-Chat-Bot/SteamChatBot/blob/master/SteamChatBot/Triggers/BaseTrigger.cs#L232)

2. [BaseTrigger.SaveTrigger(BaseTrigger trigger)](https://github.com/Steam-Chat-Bot/SteamChatBot/blob/master/SteamChatBot/Triggers/BaseTrigger.cs#L188)

3. OPTIONAL ONLY IF MAKING OWN OPTIONS: [1](https://github.com/Steam-Chat-Bot/SteamChatBot/blob/master/SteamChatBot/Triggers/BaseTrigger.cs#L855) [2](https://github.com/Steam-Chat-Bot/SteamChatBot/blob/master/SteamChatBot/Triggers/BaseTrigger.cs#L947) [3](https://github.com/Steam-Chat-Bot/SteamChatBot/blob/master/SteamChatBot/Triggers/BaseTrigger.cs#L1007) [4](https://github.com/Steam-Chat-Bot/SteamChatBot/blob/master/SteamChatBot/Triggers/BaseTrigger.cs#L1069) [5](https://github.com/Steam-Chat-Bot/SteamChatBot/blob/master/SteamChatBot/Triggers/BaseTrigger.cs#L1122) [6](https://github.com/Steam-Chat-Bot/SteamChatBot/blob/master/SteamChatBot/Triggers/BaseTrigger.cs#L1173)

**3. Add your trigger to MainWindow "Triggers" tab**
You will need to add the name of your trigger and a short description (as a ListBoxItem) to the listbox triggersListBox. Name the ListBoxItem your trigger in camelCase.

**4. Add your trigger to MainWindow.xaml.cs**
You will need to add a reference to your trigger to multiple places within MainWindow.xaml.cs in all of the following places:

1. [When the "plus" (+) button is pushed](https://github.com/Steam-Chat-Bot/SteamChatBot/blob/master/SteamChatBot/MainWindow.xaml.cs#L242)

2. OPTIONAL ONLY IF MAKING OWN OPTIONS: [Create your own GetOptions method](https://github.com/Steam-Chat-Bot/SteamChatBot/blob/master/SteamChatBot/MainWindow.xaml.cs#L242)

**5. Add your trigger to TriggerType.cs**
You need to add the name of your trigger to [TriggerType.cs](https://github.com/Steam-Chat-Bot/SteamChatBot/blob/master/SteamChatBot/Triggers/TriggerType.cs). Please make sure it is in alphabetical order with the rest of the triggers.

I know this isn't a comprehensive list, and adding a trigger is much more complicated. If you really want to see, look at past commits where I have added a trigger. Some of them I added new options [(such as AntiSpamTrigger)](https://github.com/Steam-Chat-Bot/SteamChatBot/commit/d778f9fa5b1a6c0a6ba69150feac957cb26fe2f4) or where I just used an existing options type [(like PlayGameTrigger.cs)](https://github.com/Steam-Chat-Bot/SteamChatBot/commit/f75c509502674c84251aea0259cf45a6a266138d) (ignore all the other stuff, I also worked on a few other files).

Your best bet if you want a trigger to be added is to create an issue for it and I will see about adding it in.
