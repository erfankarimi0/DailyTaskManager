
const taskList = document.getElementById("taskList");
const taskFilter = document.getElementById("taskFilter");
const createTaskButton = document.getElementById("createTaskButton");
const createTaskForm = document.getElementById("createTaskForm");
const cancelTaskButton = document.getElementById("cancelTaskButton");
const saveTaskButton = document.getElementById("saveTaskButton");
const editTaskForm = document.getElementById("editTaskForm");
const updateTaskButton = document.getElementById("updateTaskButton");
const cancelEditTaskButton = document.getElementById("cancelEditTaskButton");
const taskSearch = document.getElementById("taskSearch");
const taskSort = document.getElementById("taskSort");
const pagination = document.getElementById("pagination");
const prevPage = document.getElementById("prevPage");
const nextPage = document.getElementById("nextPage");
const pageInfo = document.getElementById("pageInfo");
let currentPage = 1;
let editingTaskId = null;

async function loadTasks() {

    let url = "/api/Tasks";

    const filter = taskFilter.value;
    const search = taskSearch.value.trim();
    const sort = taskSort.value;
    const params = new URLSearchParams();

    params.append("page", currentPage);

    if (filter !== "") {
        params.append("isCompleted", filter);
    }

    if (search !== "") {
        params.append("search", search);
    }

    params.append("sort", sort);

    if (params.toString() !== "") {
        url += `?${params.toString()}`;
    }

    try {

        const response = await fetch(url);

        if (!response.ok) {
            throw new Error("خطا در دریافت وظایف");
        }

        const result = await response.json();

        displayTasks(result.tasks);

        pageInfo.textContent =
            `صفحه ${result.currentPage} از ${result.totalPages}`;

        prevPage.disabled = result.currentPage <= 1;
        nextPage.disabled = result.currentPage >= result.totalPages;

    } catch (error) {

        taskList.innerHTML = `
            <p>خطا در دریافت وظایف</p>
        `;

        console.error(error);
    }
}


function displayTasks(tasks) {

    taskList.innerHTML = "";

    if (tasks.length === 0) {

        taskList.innerHTML = `
        <p> هیچ وظیفه‌ای پیدا نشد.</p>
            `;

        return;
    }

    const priorityText = {
        Low: "کم",
        Medium: "متوسط",
        High: "زیاد"
    };

    tasks.forEach(task => {

        const statusClass = task.isCompleted
            ? "completed"
            : "pending";

        const statusText = task.isCompleted
            ? "● انجام شده"
            : "● انجام نشده";

        const taskCard = document.createElement("div");

        taskCard.className = "task-card";

        taskCard.innerHTML = `
  <div class="task-title">
    ${task.title}
</div>



                <div class="task-info">

                    <span>
                        اولویت: ${priorityText[task.priority] ?? task.priority}
                    </span>

                    <span class="${statusClass}">
                        ${statusText}
                    </span>

<div class="task-actions">

    <button
        class="details-button"
        onclick="showTaskDetails(${task.id})">
        جزئیات
    </button>

    <button
        class="edit-button"
        onclick="showEditTaskForm(${task.id})">
        ویرایش
    </button>

    <button
        class="delete-button"
        onclick="deleteTask(${task.id})">
        حذف
    </button>

</div>

                </div>
`;

        taskList.appendChild(taskCard);
    });
}


async function showTaskDetails(id) {

    try {

        const response = await fetch(`/api/Tasks/${id}`);

        if (!response.ok) {
            throw new Error("وظیفه پیدا نشد.");
        }

        const task = await response.json();

        const priorityText = {
            Low: "کم",
            Medium: "متوسط",
            High: "زیاد"
        };

        alert(`
عنوان: ${ task.title }

توضیحات: ${ task.description ?? "-" }

اولویت: ${ priorityText[task.priority] ?? task.priority }

وضعیت: ${ task.isCompleted ? "انجام شده" : "انجام نشده" }

تاریخ سررسید: ${ task.dueDate ?? "-" }
`);

    } catch (error) {

        alert("خطا در دریافت اطلاعات وظیفه.");
        console.error(error);
    }
}

async function deleteTask(id) {

    const confirmed = confirm("آیا مطمئن هستید که می‌خواهید این وظیفه را حذف کنید؟");

    if (!confirmed) {
        return;
    }

    try {

        const response = await fetch(`/api/Tasks/${id}`, {
            method: "DELETE"
        });

        if (!response.ok) {
            throw new Error("خطا در حذف وظیفه");
        }

        loadTasks();

    } catch (error) {

        alert("خطا در حذف وظیفه.");
        console.error(error);
    }
}

async function showEditTaskForm(id) {
    editingTaskId = id;
    try {

        const response = await fetch(`/api/Tasks/${id}`);

        if (!response.ok) {
            throw new Error("وظیفه پیدا نشد.");
        }

        const task = await response.json();

        document.getElementById("editTaskTitle").value = task.title;
        document.getElementById("editTaskDescription").value = task.description ?? "";
        document.getElementById("editTaskPriority").value = task.priority;
        document.getElementById("editTaskIsCompleted").checked = task.isCompleted;

        if (task.dueDate) {
            document.getElementById("editTaskDueDate").value =
                task.dueDate.slice(0, 16);
        } else {
            document.getElementById("editTaskDueDate").value = "";
        }

        editTaskForm.style.display = "block";

        editTaskForm.scrollIntoView({
            behavior: "smooth",
            block: "start"
        });

    } catch (error) {

        alert("خطا در دریافت اطلاعات وظیفه.");
        console.error(error);
    }
}


taskFilter.addEventListener("change", () => {
    currentPage = 1;
    loadTasks();
});

taskSearch.addEventListener("input", () => {
    currentPage = 1;
    loadTasks();
});

taskSort.addEventListener("change", () => {
    currentPage = 1;
    loadTasks();
});

prevPage.addEventListener("click", () => {
    currentPage--;
    loadTasks();
});

nextPage.addEventListener("click", () => {
    currentPage++;
    loadTasks();
});

createTaskButton.addEventListener("click", () => {

    createTaskForm.style.display = "block";

});

cancelTaskButton.addEventListener("click", () => {

    createTaskForm.style.display = "none";
    document.getElementById("taskTitle").value = "";
    document.getElementById("taskDescription").value = "";
    document.getElementById("taskPriority").value = "Medium";
    document.getElementById("taskDueDate").value = "";

});


saveTaskButton.addEventListener("click", async () => {

    const title = document.getElementById("taskTitle").value;
    const description = document.getElementById("taskDescription").value;
    const priority = document.getElementById("taskPriority").value;
    const dueDate = document.getElementById("taskDueDate").value;

    const task = {
        title: title,
        description: description || null,
        isCompleted: false,
        priority: priority,
        dueDate: dueDate || null
    };



    try {

        const response = await fetch("/api/Tasks", {
            method: "POST",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify(task)
        });

        if (!response.ok) {
            throw new Error("خطا در ایجاد وظیفه");
        }

        document.getElementById("taskTitle").value = "";
        document.getElementById("taskDescription").value = "";
        document.getElementById("taskPriority").value = "Medium";
        document.getElementById("taskDueDate").value = "";

        createTaskForm.style.display = "none";

       
        loadTasks();

    } catch (error) {

        alert("خطا در ایجاد وظیفه.");
        console.error(error);
    }
});

cancelEditTaskButton.addEventListener("click", () => {

    editTaskForm.style.display = "none";

});

updateTaskButton.addEventListener("click", async () => {

    const title = document.getElementById("editTaskTitle").value;
    const description = document.getElementById("editTaskDescription").value;
    const priority = document.getElementById("editTaskPriority").value;
    const isCompleted = document.getElementById("editTaskIsCompleted").checked;
    const dueDate = document.getElementById("editTaskDueDate").value;

    const task = {
        title: title,
        description: description || null,
        isCompleted: isCompleted,
        priority: priority,
        dueDate: dueDate || null
    };

    try {

        const response = await fetch(`/api/Tasks/${editingTaskId}`, {
            method: "PUT",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify(task)
        });

        if (!response.ok) {
            throw new Error("خطا در ویرایش وظیفه");
        }

        editTaskForm.style.display = "none";

        editingTaskId = null;

        loadTasks();

    } catch (error) {

        alert("خطا در ویرایش وظیفه.");
        console.error(error);
    }

});

loadTasks();
