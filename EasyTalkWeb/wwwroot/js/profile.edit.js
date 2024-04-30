function debounce(callback, delay) {
    let timeoutId;
    return async (...args) => {
        if (timeoutId !== undefined) clearTimeout(timeoutId);
        setTimeout(async () => {
            await callback(...args);
        }, delay);
    };
}

const checkboxesContainer = document.getElementById('technologies');
const checkboxes = Array.from(checkboxesContainer.querySelectorAll('input[type="checkbox"]'));
let checkedTechnologies = checkboxes.filter(tech => tech.checked);
function sortOptions() {
    const searchInput = document.getElementById('searchTechnology').value.toLowerCase().trim();
    checkboxes.sort((a, b) => {
        const aText = a.nextElementSibling.textContent.toLowerCase();

        const bText = b.nextElementSibling.textContent.toLowerCase();
        const aRelevance = aText.includes(searchInput) ? 0 : 1;
        const bRelevance = bText.includes(searchInput) ? 0 : 1;
        if (aRelevance < bRelevance) return -1;
        if (aRelevance > bRelevance) return 1;
        return aText.localeCompare(bText);
    });
    checkboxes.forEach(checkbox => checkboxesContainer.appendChild(checkbox.parentElement));
}
const debouncedSortOptions = debounce(sortOptions, 300);
document.getElementById('searchTechnology').addEventListener('input', debouncedSortOptions);
sortOptions();

const form = document.forms.editProfileForm;
form.addEventListener('submit', async (e) => {
    e.preventDefault();
    checkedTechnologies = checkboxes.filter(checkbox => checkbox.checked).map(checkbox => checkbox.getAttribute("id"));
    const formData = new FormData(form);
    formData.set("selectedTechnologies", checkedTechnologies.join());

    await fetch('/Profile/Edit', {
        method: 'POST',
        body: formData
    });
});

const techCheckboxes = document.querySelectorAll('input.technology-checkbox');

techCheckboxes?.forEach(techCheckbox => {
    techCheckbox.addEventListener('change', () => {
        techCheckbox.parentElement.classList.toggle("checked");
    });
});