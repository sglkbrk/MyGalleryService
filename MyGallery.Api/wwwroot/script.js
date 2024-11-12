
async function fetchProjectIds() {
    try {
    const response = await fetch("/api/projects"); // Proje API URL'si
    if (!response.ok) {
        throw new Error("Proje ID'leri alınamadı");
    }
    const projects = await response.json();
    const projectsIdSelect = document.getElementById("projectsId");
    const projectsIdSelect_1 = document.getElementById("projectsId_1");
    

    projects.forEach(project => {
        const option = document.createElement("option");
        option.value = project.id; // Proje ID'sini ayarla
        option.textContent = project.title; // Proje başlığını göster
        projectsIdSelect.appendChild(option); // Seçenekleri ekle
    });
    projects.forEach(project => {
      const option = document.createElement("option");
      option.value = project.id; // Proje ID'sini ayarla
      option.textContent = project.title; // Proje başlığını göster
      projectsIdSelect_1.appendChild(option); // Seçenekleri ekle
  });
    
    } catch (error) {
    console.error(error);
    }
}
function setlogin(event) {
  if (event.key === "Enter" || event.keyCode === 13) {
    var login = document.getElementById("username").value
    localStorage.setItem("login", login)
    window.location.reload()
  }
}
function getloginParams() {
  var login = localStorage.getItem("login")
   return login.split(",");
}

async function fetchContactMe() {
  try {
      const username = getloginParams()[0]; // Kullanıcı adınızı buraya girin
      const password =  getloginParams()[1];  // Şifrenizi buraya girin
      const credentials = btoa(`${username}:${password}`); // Kullanıcı adı ve şifreyi Base64 ile kodla
      
      const response = await fetch("/api/ContantMe", {
          method: "GET",
          headers: {
              "Authorization": `Basic ${credentials}`, // Authorization başlığına ekle
              "Content-Type": "application/json"
          }
      });

      if (!response.ok) {
          throw new Error("Proje ID'leri alınamadı");
      }

      const items = await response.json();
      populateContactTable(items); // Verileri tabloya doldurma fonksiyonunuz
  } catch (error) {
      console.error(error);
  }
}
document.addEventListener("DOMContentLoaded", fetchProjectIds);
document.addEventListener("DOMContentLoaded", fetchContactMe);

document.getElementById("uploadForm").addEventListener("submit", async function(event) {
    event.preventDefault();
    const username = getloginParams()[0]; // Kullanıcı adınızı buraya girin
      const password =  getloginParams()[1];  // Şifrenizi buraya girin
      const credentials = btoa(`${username}:${password}`); 
    const formData = new FormData(this);

    const response = await fetch("/api/photo/upload", {
    method: "POST",
    headers: {
      "Authorization": `Basic ${credentials}`, // Authorization başlığına ekle
    },
    body: formData
    });

    if (response.ok) {
    const photo = await response.json();
    console.log("Fotoğraf başarıyla yüklendi:", photo);
    this.reset();
    } else {
    console.error("Fotoğraf yüklenemedi.");
    }
});

function createSlug(title) {
    return title
    .toString()
    .toLowerCase() // Küçük harflere çevir
    .trim() // Baş ve sondaki boşlukları kaldır
    .replace(/\s+/g, '-') // Boşlukları tire ile değiştir
    .replace(/[^\w\-]+/g, '') // Özel karakterleri kaldır
    .replace(/\-\-+/g, '-') // Çoklu tırnakları tek tırnağa çevir
    .replace(/^-+/, '') // Baştaki tırnağı kaldır
    .replace(/-+$/, ''); // Sondaki tırnağı kaldır
}

document.getElementById("uploadProject").addEventListener("submit", async function(event) {
    event.preventDefault();
    const username = getloginParams()[0]; // Kullanıcı adınızı buraya girin
      const password =  getloginParams()[1];  // Şifrenizi buraya girin
      const credentials = btoa(`${username}:${password}`); 
    const formData = new FormData(this);
    formData.append("slug", createSlug(document.getElementById("title").value));
    formData.append("homePage", document.getElementById("homePage").checked);
    const response = await fetch("/api/projects/upload", {
    method: "POST",
    headers: {
      "Authorization": `Basic ${credentials}`, // Authorization baisekle
    },
    body: formData
    });

    if (response.ok) {
    const photo = await response.json();
    console.log("Proje başarıyla yüklendi:", photo);
    this.reset();
    } else {
    console.error("Proje yüklenemedi.");
    }
});
function showTab(tabId) {
    const tabs = document.querySelectorAll('.tab-content');
    const buttons = document.querySelectorAll('.tab-button');

    tabs.forEach(tab => {
      tab.classList.remove('active');
    });
    buttons.forEach(button => {
      button.classList.remove('active');
    });

    document.getElementById(tabId).classList.add('active');
    const activeButton = Array.from(buttons).find(button => button.innerText.toLowerCase().includes(tabId.replace('Tab', '').toLowerCase()));
    if (activeButton) {
      activeButton.classList.add('active');
    }
  }

  // Example data for Contact Me
 

  // Function to populate the contact table
  function populateContactTable(contactData) {
    const tableBody = document.getElementById('contactTableBody');
    tableBody.innerHTML = '';  // Clear the table first
    contactData.forEach(contact => {
      const row = document.createElement('tr');
      row.innerHTML = `
        <td>${contact.id}</td>
        <td>${contact.name}</td>
        <td><a href="mailto:${contact.email}">${contact.email}</a></td>
        <td>${contact.subject}</td>
        <td>${contact.message}</td>
        <td>${contact.created}</td>
      `;
      tableBody.appendChild(row);
    });
  }
  function clearHomeCache() {
    clearCache()
  }
  function clearPostCache() {
    var projectsId = document.getElementById("projectsId_1").value
    if(projectsId) clearCache(projectsId)
  }
  async function clearCache(projectsId) {
      try {
        const username = getloginParams()[0]; // Kullanıcı adınızı buraya girin
        const password =  getloginParams()[1];  // Şifrenizi buraya girin
        const credentials = btoa(`${username}:${password}`); // Kullanıcı adı ve şifreyi Base64 ile kodla
        
        const response = await fetch("/api/Projects/ClearCache" + (projectsId ? "/" + projectsId : ""),  {
            method: "GET",
            headers: {
                "Authorization": `Basic ${credentials}`, // Authorization başlığına ekle
                "Content-Type": "application/json"
            }
        });

        if (!response.ok) {
            throw new Error("Proje ID'leri alınamadı");
        }

        const items = await response.json();
        populateContactTable(items); // Verileri tabloya doldurma fonksiyonunuz
    } catch (error) {
      
    }
  }
  // Populate the table on page load
  window.onload = populateContactTable;
  window.onload = document.getElementById("username").value = localStorage.getItem("login");