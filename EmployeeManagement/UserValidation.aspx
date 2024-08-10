<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="UserValidation.aspx.cs" Inherits="EmployeeManagement.UserValidation" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>User Validation Form</title>

    <style>
        #error {
            color: red;
        }

        .container {
            width: 400px;
            margin: auto;
            margin-top: 10%;
            border : 1px solid black;
            padding : 10px;
            box-shadow : 2px 2px 10px rgb(0, 0, 255);
        }
        input{
            padding: 5px 20px;
            margin : 5px;
            text-indent: 10px;
        }
        input:focus{
             outline: 1px solid green;
            border: 1px solid green;
        }
        #SubmitBtn{
            margin-left : 100px;
           background-color : cornflowerblue;
            color : white;
          
        }
        
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="container">
            Name:
            <input id="name" type="text" />
            <br />
            Email:
            <input id="email" type="email" />
            <br />
            Gender:
            <input id="male" name="gender" type="radio" value="Male" />
            Male
            <input id="female" name="gender" type="radio" value="Female" />
            Female
            <input id="others" name="gender" type="radio" value="Others" />
            Others
            <br />
            Age:
            <input id="age" type="number" />
            <div id="error"></div>
            <br />
            <asp:Button ID="SubmitBtn" runat="server" Text="Submit" OnClientClick="return submitForm();" />
        </div>

    </form>

    <script>
        function validateForm() {
            let name = document.getElementById('name').value.trim();
            let email = document.getElementById('email').value.trim();
            let gender = document.querySelector('input[name="gender"]:checked');
            let age = document.getElementById('age').value.trim();
            let error = document.getElementById('error');

            // Clear previous errors
            error.innerHTML = '';

            // Name validation
            if (name === '') {
                error.innerHTML = "Please enter your Name";
                return false;
            }

            // Email validation
            let emailPattern = /^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$/;
            if (email === '') {
                error.innerHTML = 'Please enter your email';
                return false;
            } else if (!emailPattern.test(email)) {
                error.innerHTML = 'Please enter a valid email';
                return false;
            }

            // Gender validation
            if (!gender) {
                error.innerHTML = 'Please select your gender';
                return false;
            }

            // Age validation
            if (age === '' || isNaN(age) || age <= 0) {
                error.innerHTML = 'Please enter a valid age';
                return false;
            }

            return true;
        }
        function submitForm() {
            if (!validateForm()) {
                return false; // Prevent form submission if validation fails
            }

            let name = document.getElementById('name').value.trim();
            let email = document.getElementById('email').value.trim();
            let gender = document.querySelector('input[name="gender"]:checked').value;
            let age = document.getElementById('age').value.trim();

            let data = {
                name: name,
                email: email,
                gender: gender,
                age: age
            };

            fetch('https://localhost:44343/Employee/AddEmployee', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(data)
            })
                .then(response => response.json())
                .then(result => {
                    if (result.Code == 201) {
                        alert('Record added successfully');
                    } else {
                        alert('Failed to add record: ' + result.message);
                    }
                })
                .catch(error => {
                    console.error('Error:', error);
                    alert('An error occurred while adding the record');
                });

            return false; // Prevent form submission
        }
    </script>
</body>
</html>
