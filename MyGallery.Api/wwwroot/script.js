let reconnectAttempts = 0; // Yeniden bağlanma denemeleri sayacı
const maxReconnectAttempts = 20; // Maksimum yeniden bağlanma denemesi
const reconnectInterval = 5000; 
let writeTime=0
let page = 1;
const pageSize = 30;
let isScrollingEnabled = true; 
const { createApp } = Vue

createApp({
    data() {
        return {
            items: [{ src: 'https://via.placeholder.com/600', w: 600, h: 400 }],
          users: [],
          onlineUsers_data: [],
          selectedUserId: null,
          messages: [],
          cachesMessages: {},
          myuser: {},
          messageText: '',
          token: '',
          connection: null,
          friendRequests: [],
          endMessageVariable: false,
          loginItem:{
            username: '',
            password: '',
            rememberMe: false,
          },       
          statusText: '',
          registerItem:{
              username: '',
              email: '',
              password: ''
          },
          registerMessage: '',
          settingsBoxVis: false,
          showEmojiPicker: false,
          darkMode: false,
          acceptType: 'image/*',
          selectedUserName: null,
        }
    },
    setup() {

    },
    mounted() {
        localStorage.getItem('theme') && document.body.setAttribute('data-theme', localStorage.getItem('theme'));
        this.token = localStorage.getItem('token');
        if (this.token) {
            this.myuser = this.parseJwt( this.token)
            this.myuser.picture =  `https://randomuser.me/api/portraits/men/${this.myuser.sub}.jpg`;
            localStorage.setItem('userId',this.myuser.sub );
            this.startConnection();
            this.loadUsers();
            sendTokenToServer();
            this.loadFriendRequests();
        }else{
            this.loginItem.username = localStorage.getItem('username');
            this.loginItem.password = localStorage.getItem('password');
            this.loginItem.rememberMe = localStorage.getItem('rememberMe');
        }
        if ("Notification" in window) {
            if (Notification.permission === "granted") {
            } else if (Notification.permission !== "denied") {
                Notification.requestPermission().then(permission => {
                    if (permission === "granted") {
                        console.log("Bildirim izni verildi.");
                    }
                });
            }
        
        }
        if(this.$refs.msgDiv){
            this.$refs.msgDiv.addEventListener('scroll', () => {
                if (this.$refs.msgDiv.scrollTop === 0 && isScrollingEnabled )  {
                    this.loadMessages(this.myuser.sub, this.selectedUserId);
                }
            });
        }  
        document.addEventListener('visibilitychange', this.handleVisibilityChange);
        document.addEventListener('click', (event) => {
            if (this.showEmojiPicker && !this.$refs.emojiPicker.contains(event.target) && event.target !== this.$refs.emojiIcon) {
                this.showEmojiPicker = false;
            }
        });
        this.$refs.emojiPicker.addEventListener('emoji-click', event => {
            this.messageText += event.detail.unicode;
        });
    },
    beforeDestroy() {
        document.removeEventListener('visibilitychange', this.handleVisibilityChange);
    },
    watch: {
        // messages: {
        //   handler(newMessages) {
        //     if(newMessages && newMessages.length)
        //     this.cachesMessages[newMessages[0].receiverId] = newMessages
        //     // Yeni mesaj eklendiğinde veya mevcut mesajlar güncellendiğinde yapılacak işlemler
        //   },
        //   deep: true // Dizi içindeki nesneleri dinlemek için gerekli
        // }
    },
    computed: {
        sortedUsers() {
            return this.sortUsersByLastMessage(this.users);
        }
    },
    methods: {
        changeTheme(){
            this.darkMode = !this.darkMode
            document.body.classList.toggle('dark-mode');
        },
        changeColor(color){
            localStorage.setItem('theme', color);
            document.body.setAttribute('data-theme', color);
        },
        handleVisibilityChange: function () {
            if(this.selectedUserId)this.sendSeen(this.selectedUserId,2);
        },
        handleKeydown: function (event) {
            if (event.key === 'Escape') {
                // ESC tuşuna basıldığında yapılacak işlemler
                console.log('ESC tuşuna basıldı');
                // Örneğin, bir modal kapatma işlemi
                // this.closeModal();
              }
        },
        async  login() {
            var json = {
                username:this.loginItem.username,
                password:this.loginItem.password
            }
            
            const response = await fetch('/api/UserAuth/login', {
                method: 'POST',
                headers: {'Content-Type': 'application/json'},
                body: JSON.stringify(json)
            });
            if (response.ok) {
                const data = await response.json();
                this.token = data.token;
                localStorage.setItem('token',  this.token);
                this.myuser = this.parseJwt( this.token);
                this.myuser.picture =  `https://randomuser.me/api/portraits/men/${this.myuser.sub}.jpg`;
                this.registerMessage = '';
                if(this.loginItem.rememberMe){
                    localStorage.setItem('username', this.loginItem.username);
                    localStorage.setItem('password', this.loginItem.password);
                    localStorage.setItem('rememberMe', this.loginItem.rememberMe);
                    localStorage.setItem('userId', this.myuser.sub );
                }else{
                    localStorage.removeItem('username');
                    localStorage.removeItem('password');
                    localStorage.removeItem('rememberMe');
                }
                this.startConnection();
                sendTokenToServer();
                this.loadUsers();
            } else {
                const error = response;
                alert('Login failed: ' + (error.statusText || 'Unknown error'));
            }
        },
        parseJwt(token) {
            const base64Url = token.split('.')[1]; // Payload kısmı
            const base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/'); // Base64 URL kodlamasını düzelt
            const jsonPayload = decodeURIComponent(escape(atob(base64))); // Base64'ten JSON'a dönüştür
            return JSON.parse(jsonPayload); // JSON'ı objeye dönüştür
        },
        async  register() {
            if (this.register.username === '' || this.register.email === '' || this.register.password === '') {
                alert('Lütfen bilgileri eksiksiz doldurun');
                return;
            }
            const response = await fetch('/api/UserAuth/register', {
                method: 'POST',
                headers: {'Content-Type': 'application/json'},
                body: JSON.stringify({ Username: this.register.username, Email: this.register.email, Password: this.register.password })
            });
            if (response.ok) {
                this.registerMessage = "Kayıt başarılı! Lütfen giriş yapın.";
                setTimeout(() => {
                    this.registerMessage = '';
                },5000)
                this.register = {
                    username: '',
                    email: '',
                    password: ''
                }
            } else {
                const error = await response.json();
                this.registerMessage = 'Kayıt başarısız: ' + (error.message || 'Lüfen Bilgileri doğrı girin ');
            }
        },
        logout(){
            this.deleteFbToken();
            localStorage.removeItem('token');
            location.reload(); 
        },
        deleteFbToken(){
            fetch('/api/FbToken/'+this.myuser.sub, { // Backend API endpoint'iniz
                method: 'DELETE',
                headers: {
                    'Content-Type': 'application/json',
                    'Authorization': `Bearer ${this.token}`,
                },
            })
            .then(response => response.json())
            .then(data => {
                console.log('Token sunucuya gönderildi:', data);
            })
            .catch((error) => {
                console.error('Token sunucuya gönderilemedi:', error);
            });
        },
        startConnection() {
            this.connection = new signalR.HubConnectionBuilder()
                .withUrl('/chathub', { accessTokenFactory: () =>  this.token })
                .build();
            this.connection.on('ReceiveMessage', (message) => {
                if (message.senderId == this.selectedUserId) {
                    var item  =  {
                        "id":message.id,
                        "senderId": message.senderId,
                        "receiverId": message.content.receiverId,
                        "content": message.content.content,
                        "timestamp": message.timestamp,
                        "status": message.content.status,
                        "type": message.content.type,
                        "fileUrl": message.content.fileUrl
                    }
                    // this.messages.push(item);
                    if(this.messages[this.messages.length - 1].senderId == item.senderId) {
                        this.messages[this.messages.length - 1].messages.push(item);
                    }else{
                        this.messages.push({
                            senderId: item.senderId,
                            messages:[item]
                        })
                    }
                    this.sendSeen(item.senderId,2);
                    setTimeout(() => {
                        this.$refs.msgDiv.scrollTop = this.$refs.msgDiv.scrollHeight;
                    }, );
                }else {
                    if(!this.isPageVisible())
                        this.showNotification(message.content.content,message.senderId,message.name,"message");
                    this.sendSeen(message.senderId,1);
                    this.setNotMessageCount(message.senderId, true);
                }   
                this.setLastMessage(message.senderId, message.content.content);
               
            });
        
            this.connection.on('ReceiveWrites', (senderId) => {
                if (senderId == this.selectedUserId) {
                    this.$refs.ReceiveWrites.style.display = 'flex';
                    setTimeout(() => {
                        this.$refs.ReceiveWrites.style.display  = 'none';
                    },3000)
                }
            });
            this.connection.on('UserStatusUpdated', (onlineUsers) => {
                this.onlineUsers_data = onlineUsers;
            });
            this.connection.on('ReceiveSeen', (userid,messageStatus) => {
                if(this.selectedUserId == userid) {
                    this.messages.forEach(msg => {
                        if(message.senderId != this.myuser.sub)
                        msg.messages.forEach(message => {
                            if (message.status != 2) {
                                message.status = messageStatus;
                            }
                        })
                    });    
                }
            });
            this.connection.on('ReceiveNotification', (data) => {
                if(data.type == "refreshFriendRequests")  this.loadFriendRequests();
                else if(data.type == "refreshUsers"){
                    this.showNotification("Arkadaş İsteğin kabul Etti",data.id,data.name,"message");
                    this.loadUsers();
                }
            });
            this.connection.on('ReceiveDeleteMessage', (userId,msgid) => {
                if(userId == this.selectedUserId){
                    this.messages.forEach(msg => {
                        msg.messages.filter(x => { if(x.id == msgid){
                           x.isDeleted = true
                           x.content = "Bu Mesaj Silindi" 
                        } } )
                   });
                }
            });
            this.connection.onclose(() => {
                this.updateConnectionStatus('Bağlantı kesildi. Yeniden bağlanma denemeleri başlatılıyor...');
                this.attemptReconnect();
            });
            this.connection.onreconnecting(() => {
                this.updateConnectionStatus('Bağlantı yeniden bağlanıyor...');
            });
            this.connection.onreconnected(() => {
                this.updateConnectionStatus('Bağlantı başarıyla yeniden kuruldu.');
            });
            this.connection.start()
                .then(() => {
                    this.updateConnectionStatus('Bağlantı başarıyla kuruldu.',3000);
                    console.log('SignalR connected')
                })
                .catch(err => console.error('SignalR connection error: ', err));
        },
        showNotification(message,senderId,name,type) {
            if ("Notification" in window && Notification.permission === "granted") {
                const notification = new Notification("Yeni Mesaj", {
                    body: `Gönderen: ${name}\nMesaj: ${message}`,
                    icon: `https://randomuser.me/api/portraits/men/${senderId}.jpg`, // İsteğe bağlı
                    data: {  senderId: senderId } 
                });
                notification.onclick = (event) => {
                    if(type == "message"){
                        window.focus();
                        this.selectUser(senderId,name);
                        notification.close();
                    }
                };
            }
        },
        isPageVisible() {
            return !document.hidden;
        },
        attemptReconnect() {
            while (reconnectAttempts < maxReconnectAttempts) {
                 new Promise(resolve => setTimeout(resolve, reconnectInterval)); // Belirli bir süre bekle
                reconnectAttempts++;
                console.log(`Yeniden bağlanma denemesi ${reconnectAttempts}...`);
                try {
                    this.connection.start(); // Yeniden bağlanmayı dene
                    console.log('Bağlantı başarıyla yeniden kuruldu.');
                    this.updateConnectionStatus('Bağlantı başarıyla yeniden kuruldu.');
                    reconnectAttempts = 0; // Başarılı olduğunda deneme sayacını sıfırla
                    return; // Yeniden bağlantı başarılıysa döngüden çık
                } catch (err) {
                    console.error('Yeniden bağlanma hatası: ', err);
                }
            }
            this.updateConnectionStatus('Maksimum yeniden bağlanma denemesi aşıldı. Lütfen internet bağlantınızı kontrol edin.',10000);
        },
        updateConnectionStatus(status,time) {
            this.statusText = status;
            setTimeout(() => this.statusText = '', time ? time: 5000);
        },
        setLastMessage(id, lastMessage) {
            const index = this.users.findIndex(user => user.id.toString() === id.toString());
            const updatedUsers = [...this.users]; // Derin kopya oluşturuyoruz
            updatedUsers[index] = {
                ...updatedUsers[index],
                lastMessage: {
                timestamp: new Date(),
                content: lastMessage
                }
            };
            this.users = this.sortUsersByLastMessage(updatedUsers);
        },
        setNotMessageCount(id, add) {
            const index = this.users.findIndex(user => user.id.toString() === id.toString());
            const updatedUsers = [...this.users]; // Derin kopya oluşturuyoruz
            updatedUsers[index] = {
                ...updatedUsers[index],
                notSeenMessagesCount: add ? updatedUsers[index].notSeenMessagesCount + 1 : 0
            };
            this.users = this.sortUsersByLastMessage(updatedUsers);
        },
        sortUsersByLastMessage(updatedUsers) {
            updatedUsers.sort((a, b) => {
                const aTimestamp = a.lastMessage ? new Date(a.lastMessage.timestamp) : new Date(0); // Varsayılan olarak 0 (1 Ocak 1970)
                const bTimestamp = b.lastMessage ? new Date(b.lastMessage.timestamp) : new Date(0); // Varsayılan olarak 0 (1 Ocak 1970)
            
                return bTimestamp - aTimestamp;
            });
            
            return updatedUsers;
        },
        async sendMessage() {
            this.addMessage(this.messageText);
            this.messageText = '';
        },
        async addMessage(messageText,type = 0,fileUrl) {
            if (this.connection && messageText) {
                messageText = this.escapeHTML(messageText);
                try {
                    var msg = {
                        SenderId: parseInt(this.myuser.sub),
                        ReceiverId: parseInt(this.selectedUserId),
                        Content: messageText,
                        Timestamp: new Date(),
                        Status: 0,
                        Type: type,
                        FileUrl: fileUrl ? fileUrl : null
                    }
                    var item  =  {
                        "senderId": msg.SenderId,
                        "receiverId": msg.ReceiverId,
                        "content": msg.Content,
                        "timestamp": msg.Timestamp,
                        "status": msg.Status,
                        "type": msg.Type,
                        "fileUrl": msg.FileUrl
                    }
                    if(this.messages && this.messages.length && this.messages[this.messages.length - 1].senderId == msg.SenderId) {
                        this.messages[this.messages.length - 1].messages.push(item);
                    }else{
                        this.messages.push({
                            senderId: msg.SenderId,
                            messages:[item]
                        })
                    }
                    this.setLastMessage(this.selectedUserId, msg.Content);
                    await this.connection.invoke('SendMessage', msg);
                    setTimeout(() => {
                        this.$refs.msgDiv.scrollTop = this.$refs.msgDiv.scrollHeight; 
                    });
                } catch (err) {
                    console.error('SendMessage error: ', err);
                }
            }else {
                this.updateConnectionStatus('Bağlantı kurulmadı. Lütfen internet bağlantınızı kontrol edin.',10000);
            }
        },
        escapeHTML(text) {
            const map = {
              '<': '&lt;',
              '>': '&gt;',
            };
            return text.replace(/[&<>"']/g, function(m) { return map[m]; });
        },
        toggleConversationArea() {
            this.$refs.conversationArea.classList.toggle('open');
        },
        sendFriendRequestBtn() {
            Swal.fire({
                title: 'Arkadaş İsteği Gönder',
                html:
                    `<input type="text" id="userName" class="swal2-input" placeholder="Kullanıcı Adı">
                     <input type="text" id="userId" class="swal2-input" placeholder="Kullanıcı ID">`,
                showCancelButton: true,
                confirmButtonText: 'Arkadaş İsteği Gönder',
                cancelButtonText: 'İptal',
                preConfirm: () => {
                    const userName = Swal.getPopup().querySelector('#userName').value;
                    const userId = Swal.getPopup().querySelector('#userId').value;
        
                    if (!userName || !userId) {
                        Swal.showValidationMessage(`Lütfen hem kullanıcı adını hem de ID'yi girin`);
                        return false;
                    }
                    
                    return { userName: userName, userId: userId };
                }
            }).then((result) => {
                if (result.isConfirmed) {
                    this.AddFriendRequest(result.value.userName, result.value.userId);
                }
            });
        },
        AddFriendRequest(userName, ReceiverId) {
            if(ReceiverId == this.myuser.sub){
                Swal.fire(
                    'Hata!',
                    'Kendinize arkadaş isteği gönderemezsiniz.',
                    'error'
                );
                return
            }
            fetch('/api/FriendRequest', { // Backend API endpoint'iniz
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'Authorization': `Bearer ${this.token}`,
                },
                body: JSON.stringify({ 
                    SenderId: parseInt(this.myuser.sub), // userId,
                    ReceiverId: parseInt(ReceiverId), // ReceiverId,
                    Status:0
                })
            })
            .then(response => response.json())
            .then(data => {
                if(data && data.id){
                    Swal.fire(
                        'Gönderildi!',
                        `${userName} (ID: ${ReceiverId}) kişisine arkadaş isteği gönderildi.`,
                        'success'
                    );
                }else{
                    Swal.fire(
                        'Hata!',
                        data.message ? data.message : 'Kullanıcı bulunamadı.',
                        'error'
                    );
                }
                
            })
            .catch((error) => {
                console.error('Arkadaş isteği gönderilemedi:', error);
            });
        },
        openFriendRequests() {
            let requestsHtml = '';
            this.friendRequests.forEach(request => {
                requestsHtml += `
                     <div class="request-item">
                        <p><i class="fas fa-user icon"></i> <strong>${request.sender.username}</strong> (ID: ${request.senderId})</p>
                        <div>
                            <button class="accept-btn" data-id="${request.id}" style="background-color: #28a745; margin-right: 5px;">
                                <i class="fas fa-check"></i> Onayla
                            </button>
                            <button class="reject-btn" data-id="${request.id}" style="background-color: #dc3545;">
                                <i class="fas fa-times"></i> Reddet
                            </button>
                        </div>
                    </div>
                `;
            });
        
            Swal.fire({
                title: 'Gelen Arkadaşlık İstekleri',
                html: requestsHtml,
                showCloseButton: true,
                showConfirmButton: false, // Onayla/Reddet butonlarını özel olarak ekliyoruz
                didOpen: () => {
                    document.querySelectorAll('.accept-btn').forEach(button => {
                        button.addEventListener('click', (event) => {
                            const requestId = event.target.getAttribute('data-id');
                            this.handleRequestAction(requestId, '1');
                        });
                    });
        
                    document.querySelectorAll('.reject-btn').forEach(button => {
                        button.addEventListener('click', (event) => {
                            const requestId = event.target.getAttribute('data-id');
                            this.handleRequestAction(requestId, '2');
                        });
                    });
                }
            });
        },
        handleRequestAction(requestId, action) {
            const request = this.friendRequests.find(req => req.id == requestId);
            fetch('/api/FriendRequest/AcceptAndRejectFriendRequest/' + request.id + "/" + action, { 
                method: 'PUT',
                headers: {
                    'Content-Type': 'application/json',
                    'Authorization': `Bearer ${this.token}`,
                },
            })
            .then(data => {
                if(data.ok){
                    if (action === '1') {
                    Swal.fire(
                        'Onaylandı!',
                        `${request.sender.username} (ID: ${request.senderId}) arkadaş olarak eklendi.`,
                        'success'
                    );
                    } else if (action === '2') {
                        Swal.fire(
                            'Reddedildi!',
                            `${request.sender.username} (ID: ${request.senderId}) arkadaşlık isteği reddedildi.`,
                            'error'
                        );
                    }
                    this.loadUsers();
                    this.loadFriendRequests();
                }        
            })
            .catch((error) => {
                console.error('Arkadaş isteği gönderilemedi:', error);
            });
        },
        openSettings() {
            this.settingsBoxVis = !this.settingsBoxVis
        },
        selectUser(receiverId,name) {
            if(receiverId == this.selectedUserId ) return
            page = 1
            this.endMessageVariable = false
            this.messages = []
            this.selectedUserId = receiverId
            this.selectedUserName = name
            isScrollingEnabled = false;
            this.loadMessages(this.myuser.sub, receiverId);
            this.$refs.conversationArea.classList.remove('open');
            this.sendSeen(receiverId,2);
            this.setNotMessageCount(receiverId, false);
        },
        async  loadUsers() {
            const response = await fetch('/api/Friend/GetChatFriends', {
                method: 'GET',
                headers: {
                    'Authorization': `Bearer ${this.token}`,
                    'Content-Type': 'application/json'
                }
            });
            if (response.ok) {
                this.users  = this.sortUsersByLastMessage( await response.json());
                this.users.forEach(user => {
                    if(user.notSeenMessagesCount > 0)this.sendSeen(user.id,1);
                })
            } else {
                if(response.status == 401) {
                    this.logout();
                }
                const error = await response.json();
                alert('Failed to load users: ' + (error.message || 'Unknown error'));
            }
        },
        async  loadMessages(senderId, receiverId) {
            // if(this.cachesMessages[receiverId]) {
            //     this.renderMessage(this.cachesMessages[receiverId],receiverId);
            //     return
            // }
            if(this.endMessageVariable) return
            const response = await fetch('/api/message/GetMyMessages/' + senderId + '/' + receiverId + '/'+ page +'/' + pageSize, {
                method: 'GET',
                headers: {
                    'Authorization': `Bearer ${this.token}`,
                    'Content-Type': 'application/json'
                }
            });
            if (response.ok) {
                var messages = await response.json()
                var arr = [];
                messages.forEach(msg => {
                    if(msg.senderId != arr[arr.length - 1]?.senderId) {
                        arr.push({
                            senderId: msg.senderId,
                            messages:[msg]
                        })
                    }else {
                        arr[arr.length - 1].messages.push(msg)
                    }
                })
                this.renderMessage(arr,receiverId);
            } else {
                const error = await response.json();
                alert('Failed to load users: ' + (error.message || 'Unknown error'));
            }
        },
        renderMessage(messages,receiverId) {
            const msgDiv2 = this.$refs.msgDiv;
            var sy = msgDiv2.scrollHeight;
            if(messages.length == 0 && page > 1) {
                this.endMessageVariable = true
            } 
            this.messages  =  messages.concat(this.messages)
            this.cachesMessages[receiverId] = this.messages;
            setTimeout(() => {
                if(page ==1) msgDiv2.scrollTop = msgDiv2.scrollHeight; 
                else  msgDiv2.scrollTop = msgDiv2.scrollHeight - sy;
                isScrollingEnabled = true;
                page++;
            }, 100);
        },

        async  onChangeMsg () {
            if(writeTime  == 0 ){
                await this.connection.invoke('SendWrite', this.selectedUserId.toString());
                writeTime = 3000
                setTimeout(() => {
                    writeTime = 0
                },writeTime)
            }
           
        },
        async  sendSeen (userId,messageStatus) {
            if(this.connection){
                if(!this.isPageVisible()) messageStatus = 1
                await this.connection.invoke('SendSeen', userId.toString(), messageStatus);
            }
        },
        async  loadFriendRequests() {
            const response = await fetch('/api/FriendRequest/GetFriendRequestByReceiverId/' + this.myuser.sub, {
                method: 'GET',
                headers: {
                    'Authorization': `Bearer ${this.token}`,
                    'Content-Type': 'application/json'
                }
            });
            if (response.ok) {
                this.friendRequests = await response.json();    
            } else {
                if(response.status == 401) {
                    logout();
                }
                const error = await response.json();
                alert('Failed to load users: ' + (error.message || 'Unknown error'));
            }
        },
        timeAgo(isoDateString) {
            const turkishMonths = [
                'Oca', // Ocak
                'Şub', // Şubat
                'Mar', // Mart
                'Nis', // Nisan
                'May', // Mayıs
                'Haz', // Haziran
                'Tem', // Temmuz
                'Ağu', // Ağustos
                'Eyl', // Eylül
                'Eki', // Ekim
                'Kas', // Kasım
                'Ara'  // Aralık
            ];
        
            // ISO 8601 tarih string'ini Date objesine dönüştürme
            const date = new Date(isoDateString);
        
            // Gün bilgisini alma
            const day = date.getDate();
        
            // Ay bilgisini Türkçe kısaltmalarla alma
            const month = turkishMonths[date.getMonth()];
        
            // Saat bilgisini alma ve iki haneli yapma
            const hours = date.getHours().toString().padStart(2, '0');
        
            // Dakika bilgisini alma ve iki haneli yapma
            const minutes = date.getMinutes().toString().padStart(2, '0');
        
            // İstenen formatta birleştirme
            return `${day} ${month} ${hours}:${minutes}`;
        },
        toogleEmojiBox() {
            this.showEmojiPicker = !this.showEmojiPicker
        },
        triggerFileSelect() {
            this.acceptType = '*';
            setTimeout(() => {
                this.$refs.fileInput.click();
            }, 100);
            
        },
        triggerImageSelect() {
            this.acceptType = 'image/*'; 
            setTimeout(() => {
                this.$refs.fileInput.click();
            }, 100);
        },
        handlePaste(event) {
            // Yapıştırılan içerikleri al
            const items = event.clipboardData.items;
            for (let i = 0; i < items.length; i++) {
              const item = items[i];
              // Eğer yapıştırılan bir resim dosyasıysa
              if (item.kind === 'file' && item.type.startsWith('image/')) {
                const file = item.getAsFile();
                // Dosyayı doğrudan handleFileChange'e gönder
                this.handleFileChange({ target: { files: [file] } });
              }
            }
        },
        onImageLoad() {
            // this.$refs.msgDiv.scrollTop = this.$refs.msgDiv.scrollHeight; 
        },
        downloadFile(fileUrl) {
            const link = document.createElement('a');
            link.href = '/api/MinioFile/download/' + fileUrl;
            link.download = fileUrl.split('/').pop();
            document.body.appendChild(link);
            link.click();
            document.body.removeChild(link);
        },
        async handleFileChange(event) {
            const file = event.target.files[0];
            if(file.size > 1024 * 1024 * 5) {
                alert('Dosya boyutu 5MB den uzun olamaz!');
                return
            }
            const formData = new FormData();
            formData.append('file', file);
            fetch('/api/MinioFile/upload', {
                method: 'POST',
                headers: {
                    'Authorization': `Bearer ${this.token}`,
                },
                body: formData,
            })
            .then((response) => {
                if (!response.ok) {
                    throw new Error('Dosya yükleme başarısız!');
                }
                return response.json();
            })
            .then((data) => {
                var type = this.acceptType == 'image/*' ? 1 : 2;
                this.addMessage(file.name, type, data.key);
                console.log('Dosya başarıyla yüklendi:', data);
            })
            .catch((error) => {

                console.error('Hata:', error);
            });
        },
        async deleteMessage(receiverId,id){
            if(this.connection){
                await this.connection.invoke('DeleteMessage', receiverId.toString(), id);
                if(receiverId == this.selectedUserId){
                    this.messages.forEach(msg => {
                         msg.messages.filter(x => { if(x.id == id){
                            x.isDeleted = true
                            x.content = "Bu Mesaj Silindi" 
                         } } )
                    });
                }
            }
        }

    }
}).mount('#projetApp')