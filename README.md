# Project Management Website [ Canny ]

This website includes the tools and techniques required to deliver your projects successfully. It gives customers to create Story
board for their requirements and according to that story board team members and admin create SRS and finalize the SRS which customer can see. Customer can also comment on SRS so that it can be tweaked. Also all the users can chat with each other using chat functionality. Customers can also create posts and all the users can comment and upvote on those posts. according to the posts admin creates the issues of the project and assign the issues to the team members. Admin also creates the gant chart so that all the team members can see the timeline of the project. All users can change their profiles acording to their needs.

## Prerequisite

- **ASP.NET MVC** and **Visual Studio 2015**
- **MS SQL Server** as database back-end.
  - **Entity Framework 6** to access your database.
  - Perform all the **migrations** to create tables in Visual Studio.

### Steps to execute project

3. Create an empty project in Visual Studio and copy all the files listed in the **.gitignore** file.

4. Go to nuGet package Manager Console and add/update following dependencies :

- Signal R
- Owin

3. Go to PM Console and type following command:

```
> Enable-Migrations
> Add-Migrations InitialCreate
> Update Database
```

4. Buid the project using ` Ctrl+Shift+B`

5. Run the project using `Ctrl+F5`

6. The Project will start in the browser on the following link

```
http://localhost:44300
```

## Screenshots

</br>

![](screenshot/frontpage.png)
![](screenshot/frontpage2.png)
![](screenshot/login.png)
![](screenshot/register.png)
![](screenshot/profile.png)
![](screenshot/createProject.png)
![](screenshot/projects.png)
![](screenshot/addteam_cust.png)
![](screenshot/addedsuccess.png)
![](screenshot/chatbox.png)
![](screenshot/createStoryBoard.png)
![](screenshot/createSRS.png)
![](screenshot/addissue.png)
![](screenshot/addissue1.png)
![](screenshot/assignIssue.png)
![](screenshot/Questions.png)
![](screenshot/Task.png)
![](screenshot/gantt.png)

## Authors

- **Swar Patel** -
  _Profile Management,Error Handling,Issue Assign Management,Team Member Management,SRS and Story Board Management,Project Creation Handling_ - [Swar Patel](https://github.com/swarpatel23)
- **Vyom Pathak** -
  _Authenticaltion and Autorization Handling,External Login,Chat Functionality,Issue Management_ - [Vyom Pathak](https://github.com/01-vyom)
- **Priyank Chaudhari** -
  _Whole Project CSS and Javascript,Board management,Posts and Task Management,Roadmap Management_ - [Priyank Chaudhari](https://github.com/pc810)
