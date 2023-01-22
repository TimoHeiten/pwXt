## pwXt

Cli to store your passwords in a litedb.
Kind of a backend for password manager solutions.

You can CRUD passwords via a key and value and they are encrypted with salt and a passphrase you have to specify in the appsettings.json file

```
{
    "Config" :{
        "Salt" : "any-secret-salt",
        "Passphrase" : "swordfish",
        "ConnectionString" : "Filename=./pw-store.db; Connection=Shared;"
    }
}
```

Make sure that the passphrase is known only to you, and is safely stored or memorized and only used if you use the app itself. You can also set it via an ENVIRONMENT Variable which overrides the .NET appsettings like so:

Windows with powershell:
```[Environment]::SetEnvironmentVariable("PW_XT__Config__Passphrase", $env:Path + "swordfish2", "Machine")```


bash for linux or macOS
```
# open bash profile 
$EDITOR ~/.profile
# add line somewhere:
export PW_XT__Config__Passphrase=swordfish2
```


## Commands overview
Is returned when you execute the CLI with the '--help' option:
```
pwXt-cli v1.0.0

USAGE
  pwXt-cli <operation> <key> [options]
  pwXt-cli [command] [...]

DESCRIPTION
  Mutate - (add, alter, del) -  a password in the password store

PARAMETERS
* operation         The operation to perform on the password store (add, remove, update)
* key               The key to store the password under

OPTIONS
  -v|--value        The password to store
  -h|--help         Shows help text.
  --version         Shows version information.

COMMANDS
  create-db         Create the lite db file at the specified path
  get               Get a password from the password store via the specified key
  list              List all password keys in the password store
  merge             merge multiple litedb files into one

You can run `pwXt-cli [command] --help` to show help on a specific command.

```
## Flow to create passwords

#### Create the database file
(either use the build .exe or the dotnet CLI to invoke the commands. Here we use dotnet CLI, the '--' separates dotnet cli commands (like targetframework, dll to run etc.) from your application arguments

```dotnet run -- create-db $file-to-db```
output:
```created db and passwords collection and copied path to clipboard```
The copied file-path you should then paste in the appsettings.json in the ConnectionString value (after Filename and before first semicolon);

### Create a first key
```dotnet run -- add key-1 -v password1```

![image](https://user-images.githubusercontent.com/20025919/213924473-c5fef84b-339c-42d4-bace-e17155f70b1e.png)


### Read the Key back
```dotnet run -- get key-1```

![image](https://user-images.githubusercontent.com/20025919/213924573-79adde0f-0865-426b-b3d5-316518614373.png)

### Update the Password
``` dotnet run -- alter key-1 -v neues-password ```

![image](https://user-images.githubusercontent.com/20025919/213924596-34d85157-3eb3-433b-b4fa-f650ca5f1df8.png)

### Delete the Password 
```dotnet run -- del key-1```

![image](https://user-images.githubusercontent.com/20025919/213924643-7f4cc5e0-ba5f-42cb-bf5d-2348d8a1836c.png)

