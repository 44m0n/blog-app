# Welcome to my portfolio!

Hi! This is a demo application that I created for my portfolio.  The app is divided into 3 projects: MVC (with Razor), API and an Angular app that uses the API project.


# Installation Guide

Clone the repository, open the solution and run the **BlogApp.Dotnet.WE<span><span>B** project. This will seed the database and launch the browser.

## BlogApp.Dotnet.WE<span><span>B

This is a fully functional web app with authorization and authentication. You can log in/register as a user and create new posts, comments or replies. Also you can edit/delete your own content (posts, replies, comments). As an admin you can modify all the information. Also, you can see the information of other users and edit/delete it. 

**Credentials**
- admin
	> email: admin@mail.<span><span>com
	>password: admin
- user
	>email: testuser@mail.<span><span>com
	>password: testuser
- user
	>email: testuser2@mail.<span><span>com
	>password: testuser2
	

## BlogApp.Dotnet.API

This is a fully functional API project. The authorization and authentication are still in development in another branch. 

## BlogAppSPA

This project is made with Angular and uses **BlogApp.Dotnet.API** project to run. The project is still under development and it lacks some things (like error handling or tests), but it is also fully functional (on happy path scenarios). 

**Installation**
- go to the project root folder, open a terminal and run `npm install` to install all the packages.
- run `ng serve --open` to start the application and launch default browser

The Angular app will make request to `http://localhost:5000` address, so make sure you run BlogApp.Dotnet.API project on **BlogApp.Dotnet.WebAPI** profile instead of **IIS Express**. Otherwise, change the address in `BlogAppSPA/environments`.