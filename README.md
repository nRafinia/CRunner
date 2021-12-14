# Command-Runner
Easy write commands and run multiple server

### Yamel sample:
```
  Address:
    IP:
      192.168.1.1:
  Security:
    UserName: admin
    Password: admin
  Commands:
    Lines:
    - enable
    - admin
    - show run int gi0/1
    - sleep:3000
    - wr
    - sleep:3000
```
