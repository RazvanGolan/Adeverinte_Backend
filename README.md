# School Certificate Issuance

## Overview

This repository contains the backend implementation for a school certificate issuance system. It was made to be integrated with the faculty's website <a href="https://student.upt.ro">student.upt</a>

The frontend of this project can be found <a href="https://github.com/RaulCandrea/AdeverinteFrontendFIS">here</a>

<h2>Student's Dashbord</h2>
  <li><b>Request Certificates:</b> Students can easily request new certificates.</li>
  <li><b>View Completed Certificates:</b> Students can view the status and details of their requested certificates.</li>

<h2>Secretary's Dashbord</h2>
  <li><b>Sort and Manage Requests:</b> Secretaries can sort certificate requests thoroughly.</li>
  <li><b>Approve or Reject Requests:</b> Secretaries can approve or reject certificate requests.</li>
  <li><b>Sign Certificates:</b> Approved certificates are signed physically and uploaded to the site.</li> 
  <li><b>Email Notifications:</b> Students receive email notifications for both rejected and approved certificates.</li>

## Technical details
<details>
<summary>Technology Stack</summary>
<ul>
  <li><strong>Backend Framework:</strong> .NET Core 8, ASP.NET Core 8</li>
  <li><strong>ORM:</strong> Entity Framework Core</li>
  <li><strong>Database:</strong> PostgreSQL</li>
  <li><strong>API Documentation:</strong> Swagger UI</li>
  <li><strong>Authentication & Storage:</strong> Firebase</li>
</ul>
</details>
  
<details>
<summary>Libraries Used</summary>
<ul>
  <li><strong>QuestPDF:</strong> For generating PDFs dynamically.</li>
  <li><strong>MailKit:</strong> For sending emails.</li>
</ul>
</details>

<details>
<summary>Data Models</summary>

### Student
```C#
  public class Student
{
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public FacultyModel Faculty { get; private set; }
    public SpecialityModel Speciality { get; set; }
    public string Email { get; private set; }
    public RoleEnum Role { get; private set; } // Secretary is just a student with a different role
    public int Year { get; private set; }
    public string Marca { get; private set; }
}
```

### Certificate
```C#
  public class Certificate
{
    public string Text { get; private set; }
    public bool OnEmail { get; private set; }
    public StudentModel Student { get; private set; }
    public TypeEnum Type { get; private set; }
    public string Motive { get; private set; }
    public StateEnum State { get; private set; }
    public DateTime Created { get; private set; }
    public DateTime? Accepted { get; private set; }
    public string? Number { get; private set; }
    public string? RejectMsg { get; private set; }
    public PdfModel? Pdf { get; private set; }
}
```
</details>

## Workflow

<ol>
  <li><b>Student Requests Certificate:</b> The student fills out the necessary information and requests a certificate through the dashboard.</li>
  <li><b>Certificate Generation:</b> The backend dynamically generates the certificate using the provided data.</li> 
  <li><b>Secretary Review:</b> The secretary reviews the request, sorts them, and decides to accept or reject each request.</li>
  <li><b>Notification:</b> If rejected, an email is sent to the student with the rejection reason. If accepted, the certificate is signed physically, uploaded, and an email with the signed document is sent to the student.</li>
</ol> 

## Getting Started

1. **Clone the repository:**
```sh
git clone https://github.com/RazvanGolan/school-certificate-issuance-backend.git
```

2. **Navigate to the project directory:**
```sh
cd school-certificate-issuance-backend
```
3. **Set up the database:**
- Ensure PostgreSQL is installed and running.
- Update the connection string in the `appsettings.json` file.

4. **Run the project:**
```sh
dotnet watch run
```
