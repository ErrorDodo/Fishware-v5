# Fishware-csgo
An Account Checker/Viewer for VALVE'S CSGO 

# Ever wanted a program that checks CSGO accounts?
### Neither but here it is.
(I have actually lost all the screenshots so this will have to do)

Sick and tired of having to manually sign into your non prime accounts you buy in order to play sweaty against the low ranks, well is this is program for you. Just input a text file that looks something like this (They are working accounts dont tell anyone (I think));
```
j121_5XwFTB:zbECi4dB
j121_TRJRo1:Jl9sqFHf
j121_BE7xZm:hA6vkEKb
j121_RzFN9l:b5TfQaqv
```
During the checking the program will give you real time logging

After the masterful and totally not broken logging has been shown navigate to the view accounts tab

Where you will see nicely formatted information about each account (that it can log into)
The general idea of the formatting was this;

Profile Pic | Display Name | Username | Wins | Level | Profile link
------------|--------------|----------|------|-------|-------------

The program has extra options when an account is right clicked on (Make sure you left click it first)
- Double left click will take you to the accounts steam profile link


Program also allows for customisation of almost every colour element (If you ever felt like making this amazing software look hideous);

And uhhh, the colours do stick, so make sure you like the colours otherwise its back the square one with the reset colour.

# How to use
In order to make the most of the programs account checking options you must use a steam API key from here (https://steamcommunity.com/dev/apikey)
- How to change the API
  - Navigate to BanCheck.cs in Fishwarev5/DataEntry
  - Go to line 270 where ```csharp string api_key``` is referenced 
  - Get API key from said link and paste it
  - Start Checking
  - pssst. I have left my steam api key in it (It won't work due to being revoked though :P)


#To Do
- [ ] Create better SQL functions
- [ ] Comment to make it easier for people to read
- [ ] Fix the bug where accounts are logged twice
- [ ] Allow messages to keep being displayed to logging window even when clicking on view/home tabs
- [x] Pretend its not version 5 


# Required
https://dotnet.microsoft.com/download/dotnet-framework/net472
