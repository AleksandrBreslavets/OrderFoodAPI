const uri = 'api/Dishes';
const cat_uri = 'api/Categories';
const nullDescr = "Немає опису";
let dishes = [];

function getDishes() {
    fetch(uri)
        .then(response => response.json())
        .then(r => _displayDishes(r.data))
        .catch(error => console.error('Неможливо отримати страви.', error));
}

function getCategories() {
    fetch(cat_uri)
        .then(response => response.json())
        .then(r => _fillCategories(r.data))
        .catch(error => console.error('Неможливо отримати категорії.', error));
}

function addDish() {
    const addNameTextbox = document.getElementById('add-name');
    const addInfoTextbox = document.getElementById('add-desctiption');
    const addWeightTextbox = document.getElementById('add-weight');
    const addPriceTextbox = document.getElementById('add-price');
    const addCaloriesTextbox = document.getElementById('add-calories');
    const addCategorySelect = document.getElementById('add-category');


    const dish = {
        name: addNameTextbox.value.trim(),
        description: addInfoTextbox.value.trim(),
        weight: addWeightTextbox.value.trim(),
        price: addPriceTextbox.value.trim(),
        calories: addCaloriesTextbox.value.trim(),
        categoryId: addCategorySelect.value
    };

    fetch(uri, {
        method: 'POST',
        headers: {
            'Accept': 'application/json',
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(dish)
    })
        .then(response => response.json())
        .then(() => {
            getDishes();
            addNameTextbox.value = '';
            addInfoTextbox.value = '';
            addWeightTextbox.value = '';
            addPriceTextbox.value = '';
            addCaloriesTextbox.value = '';
        })
        .catch(error => console.error('Неможливо додати страву.', error));
}

function deleteDish(id) {
    fetch(`${uri}/${id}`, {
        method: 'DELETE'
    })
        .then(() => getDishes())
        .catch(error => console.error('Неможливо видалити страву.', error));
}

function displayEditForm(id) {
    const dish = dishes.find(d => d.dishId === id);

    document.getElementById('edit-id').value = dish.dishId;
    document.getElementById('edit-name').value = dish.name;
    document.getElementById('edit-desctiption').value = dish.description;
    document.getElementById('edit-weight').value = dish.weight;
    document.getElementById('edit-price').value = dish.price;
    document.getElementById('edit-calories').value = dish.caloriesNumber;
    document.getElementById('edit-category').value = dish.categoryId;
    document.getElementById('editForm').style.display = 'block';
}

function updateDish() {
    const dishId = document.getElementById('edit-id').value;
    const dish = {
        id: parseInt(dishId, 10),
        name: document.getElementById('edit-name').value.trim(),
        description: document.getElementById('edit-desctiption').value.trim(),
        weight: document.getElementById('edit-weight').value.trim(),
        price: document.getElementById('edit-price').value.trim(),
        calories: document.getElementById('edit-calories').value.trim(),
        categoryId: document.getElementById('edit-category').value.trim()
    };

    fetch(`${uri}/${dishId}`, {
        method: 'PUT',
        headers: {
            'Accept': 'application/json',
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(dish)
    })
        .then(() => getDishes())
        .catch(error => console.error('Неможливо оновити страву.', error));

    closeInput();

    return false;
}

function closeInput() {
    document.getElementById('editForm').style.display = 'none';
}


function _displayDishes(data) {
    const tBody = document.getElementById('dishes');
    tBody.innerHTML = '';


    const button = document.createElement('button');

    data.forEach(d => {
        let editButton = button.cloneNode(false);
        editButton.innerText = 'Редагувати';
        editButton.setAttribute('onclick', `displayEditForm(${d.dishId})`);

        let deleteButton = button.cloneNode(false);
        deleteButton.innerText = 'Видалити';
        deleteButton.setAttribute('onclick', `deleteDish(${d.dishId})`);

        let tr = tBody.insertRow();


        let td1 = tr.insertCell(0);
        let link = document.createElement("a");
        link.href = "details.html?id=" + d.dishId;
        link.textContent = d.name;
        td1.appendChild(link);

        let td2 = tr.insertCell(1);
        const descr = d.description ? d.description : nullDescr;
        let textNodeInfo = document.createTextNode(descr);
        td2.appendChild(textNodeInfo);

        let td3 = tr.insertCell(2);
        let textNodeWeight = document.createTextNode(d.weight);
        td3.appendChild(textNodeWeight);

        let td4 = tr.insertCell(3);
        let textNodePrice = document.createTextNode(d.price);
        td4.appendChild(textNodePrice);

        let td5 = tr.insertCell(4);
        let textNodeCalories = document.createTextNode(d.caloriesNumber);
        td5.appendChild(textNodeCalories);

        let td6 = tr.insertCell(5);
        let textNodeCategory = document.createTextNode(d.category);
        td6.appendChild(textNodeCategory);

        let td7 = tr.insertCell(6);
        td7.appendChild(editButton);

        let td8 = tr.insertCell(7);
        td8.appendChild(deleteButton);
    });

    dishes = data;
}

function _fillCategories(data) {
    const add_select = document.getElementById('add-category');
    const edit_select = document.getElementById('edit-category');
    data.forEach(c => {
        const opt_add = document.createElement('option');
        opt_add.value = c.categoryId;
        opt_add.textContent = c.name;
        add_select.appendChild(opt_add);

        const opt_edit = document.createElement('option');
        opt_edit.value = c.categoryId;
        opt_edit.textContent = c.name;
        edit_select.appendChild(opt_edit);
    });
}