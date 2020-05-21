# ChatGroups
ChatGroups is a test task for NG interview process; contains 4 projects:
* ChatGroups - server, which provides the functionality to chat in groups. Clients may create, join, list and leave chat groups. 
* ChatGroupsContracts - shared between server and client(s) contracts and models. 
* ChatGroupsTests - project which contains tests for ChatGroups code. 
* Client - console client, implemented for testing server functionality.

Server is hosted on Azure as AppService, you may find it by [this link](http://chatgroups.azurewebsites.net).

# Contracts
Please refer to **ChatGroupsContracts** when connecting your client to **ChatGroups**.

**IServerHubContract** represents available methods on server Hub, which can be called by clients. 

**IClientHubContract** Represents methods which should be implemented on client side to work properly with all possible functionality (create/join/list/leave groups). 


# Working with test client
Run Console Application Client. Currently it's set up to worh with server on Azure. Please change *connectionUrl* in *Program.cs* if you would like to refer to local version of server. 

To signUp you need to provide your nickname, choose whatever your like. 
*Please note that it's just a placeholder, assuming that real auth module is already implemented.* 
![](https://i.ibb.co/xz0ZgLN/image.png)


After signUp process you will be able to work with groups. By default a new client is not a member of any group, so please either create a new one or join an existing one. You may also retrieve a list of groups from main menu, so you will be able to pick up ID of a group you would like to join.

*NOTE: Those IDs are not user-friendly, since it's not defined for now and used for testing purposes. In the future there might be groupIds used like user-tags in games (example: [name]#number) or groups with unique name only, or any other requirement. For now please copy the GUID you get to join the group you would like to*
![](https://i.ibb.co/CWhQ5S4/image.png)

You would receive messages in real time from all groups you're a member of. However you may write to one group at a time. To change current group, please select proper menu option. If there's no current group selected for you, you wouldn't be able to send a message.


# Developer notes
* Considerations and code-related notes are marked as corresponding comments (marked as TBD and NOTE). 
* As an improvement, I'd like to split more the responsibilities of ChatHub, since it looks a bit huge. I'd also love to use Automapper instead of creating new models from existing ones. 
* I would assume that all client-related functionality (like ClientRepository, models and SignUp method) would be separated from current implementation, since it's not a responsibility for chat groups. Just done for testing purposes.
* Since Client is created to verify server funcationality, please mind that its' implementation was not my main focus. Program.cs, Processor.cs could be refactored, as well as menu could be done in a more user-friendly way, tests could also be done. 

Hope you find it useful, have fun in chats! :)




