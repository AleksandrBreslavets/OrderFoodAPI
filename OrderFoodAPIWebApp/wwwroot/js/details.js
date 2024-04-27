const uri = 'api/Dishes';

function getDishById() {
    const urlParams = new URLSearchParams(window.location.search);

    const dishId = urlParams.get('id');

    fetch(`${uri}/${dishId}`)
        .then(response => response.json())
        .then(r => _displayDishById(r.data))
        .catch(error => console.error('Неможливо отримати страву.', error));
}

function _displayDishById(dish) {
    const d_object = {
        name_h1: document.getElementById('name'),
        weight_p: document.getElementById('weight'),
        price_p: document.getElementById('price'),
        calories_p: document.getElementById('calories'),
        category_p: document.getElementById('category'),
        restaurants_p: document.getElementById('restaurants'),
        descr_p: document.getElementById('descr')
    }

    const restaurants_str = dish.restaurants.length > 0 ? dish.restaurants.join(", ") : "відсутні ресторани";
    const descr_str = dish.description ? dish.description : "немає опису";

    d_object.name_h1.textContent = dish.name;
    d_object.weight_p.textContent = `${dish.weight} гр`;
    d_object.price_p.textContent = `${dish.price} грн`;
    d_object.calories_p.textContent = `${dish.caloriesNumber} (на 100 гр страви)`;
    d_object.category_p.textContent = dish.category;
    d_object.restaurants_p.textContent = restaurants_str;
    d_object.descr_p.textContent = descr_str;

}