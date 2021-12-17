# Command Runner (CRunner)
Easy write commands and run multiple server

<p align="center" width="100%">
  <img src="/images/crunner.png"> 
</p>
  
### Yamel sample:
```
  Address:
    Include: setting-include.yml
    IP:
      192.168.1.1:
        Mode: Telnet
      192.168.20.1:
        Mode: SSH
          Security:
            UserName: admin2
            Password: 1234
  Security:
    UserName: admin
    Password: admin
  Commands:
    Include: setting-include.yml
    Lines:
    - enable
    - admin
    - show run int gi0/1
    - sleep:3000
    - wr
    - sleep:3000
```

