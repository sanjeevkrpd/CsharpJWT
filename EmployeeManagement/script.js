
// Validate and Submit Form
let btn = document.getElementById('SubmitBtn');

btn.addEventListener("click", (event) => {
    if (validateForm()) {
        submitForm();
    }
});

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

async function submitForm() {
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
    const url = "https://localhost:44343/Employee/AddEmployee";
    const token = localStorage.getItem("token");
    try {
        let res = await fetch(url, {
            method: 'POST',
            headers: {
                'Authorization': `Bearer ${token}`,
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(data)
        });

        const responseData = await res.json();
        console.log(responseData.success)
        if (responseData.success) {
            document.getElementById('error').innerHTML = '';
            document.getElementById('success').innerHTML = 'Authentication & Data Fetched Successfully';
            clearForm(); // Clear form on success
        } else {
            document.getElementById('error').innerHTML = "Something went wrong";
        }

    } catch (error) {
        console.log("ERROR: " + error);
        document.getElementById('error').innerHTML = "An error occurred while Adding employee data.";
    }
}

function clearForm() {
    document.getElementById('name').value = '';
    document.getElementById('email').value = '';
    document.getElementById('age').value = '';
    document.querySelectorAll('input[name="gender"]').forEach(radio => radio.checked = false);
}

// Login Functionality
let Loginbtn = document.getElementById('LoginBtn');
Loginbtn.addEventListener("click", async () => {
    let username = document.getElementById("user").value;
    let password = document.getElementById("password").value;
    let success = document.getElementById('Logsuccess');
    let error = document.getElementById('Logerror');

    success.innerHTML = '';
    error.innerHTML = '';

    let obj = {
        username: username,
        password: password
    };

    let urlLogin = "https://localhost:44343/Employee/Login";

    try {
        const response = await fetch(urlLogin, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(obj)
        });

        if (!response.ok) {
            throw new Error('Network response was not ok');
        }

        const resData = await response.json();

        if (resData.success) {
            success.innerHTML = "Login Successfully";
            localStorage.setItem("token", resData.token);
        } else {
            error.innerHTML = "Invalid User or Password!";
        }

    } catch (error) {
        console.error('Error:', error);
        error.innerHTML = 'An error occurred during login';
    }
});




let dispBtn = document.getElementById('DispBtn');
let success = document.getElementById('disp_success');
let error = document.getElementById("disp_error");

dispBtn.addEventListener("click", async () => {
    let urlListEmployee = "https://localhost:44343/Employee/ListEmployee";
    let token = localStorage.getItem("token");

    console.log(token);

    try {
        const response = await fetch(urlListEmployee, {
            method: 'POST',
            headers: {
                'Authorization': `Bearer ${token}`
            }
        });

        if (!response.ok) {
            error.innerHTML = 'Network response was not ok';
            throw new Error('Network response was not ok');
        }

        const resData = await response.json();

        // Log response data for debugging
        console.log('Response Data:', resData);

        // Ensure `empList` is accessed correctly and is an array
        const empList = resData?.data?.Data?.empList;

        if (resData.message === "success" && Array.isArray(empList)) {
            displayEmployeeList(empList);
            success.innerHTML = 'Authentication & Data Fetched Successfully';
        } else {
            error.innerHTML = "Something went wrong";
        }
    } catch (error) {
        console.error('Error:', error);
        error.innerHTML = "An error occurred while fetching employee data.";
    }
});

function displayEmployeeList(empList) {
    const container = document.getElementById('employeeListContainer');
    container.innerHTML = ''; // Clear any existing content

    empList.forEach(emp => {
        // Create a new div for each employee
        const empDiv = document.createElement('div');
        empDiv.classList.add('employee');

        // Create a string with the employee details
        const empDetails = `
            <p><strong>ID:</strong> ${emp.id}</p>
            <p><strong>Name:</strong> ${emp.name}</p>
            <p><strong>Email:</strong> ${emp.email}</p>
            <p><strong>Gender:</strong> ${emp.gender}</p>
            <p><strong>Age:</strong> ${emp.age}</p>
            <button onclick="deleteEmp(${emp.id}, this.parentElement)">Delete</button>
            <hr>
        `;

        // Set the innerHTML of the div
        empDiv.innerHTML = empDetails;

        // Append the div to the container
        container.appendChild(empDiv);
    });
}

async function deleteEmp(empId, empDiv) {
    console.log(empId);
    let url = "https://localhost:44343/Employee/DeleteEmployee";
    let token = localStorage.getItem("token");

    console.log(token);

    try {
        const response = await fetch(url, {
            method: 'POST',
            headers: {
                'Authorization': `Bearer ${token}`,
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({ "id": empId })
        });

        if (!response.ok) {
            error.innerHTML = 'Network response was not ok';
            throw new Error('Network response was not ok');
        }

        const resData = await response.json();

        console.log('Response Data:', resData);

        if (resData.success) {
            // Remove the empDiv element from the DOM
            empDiv.remove();
            success.innerHTML = 'Authentication & Data Deleted Successfully';
        } else {
            error.innerHTML = "Something went wrong";
        }
    } catch (error) {
        console.error('Error:', error);
        error.innerHTML = "An error occurred while Deleting employee data.";
    }
}
