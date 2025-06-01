# Data Folder

This folder contains data and configuration files used by the PastryTycoon project. Its contents are primarily intended for local development and testing purposes.

## Structure

- **azurite/**  
  Contains data files for the Azurite local Azure Storage emulator.  
  - `__azurite_db_queue__.json`, `__azurite_db_queue_extent__.json`, `__azurite_db_table__.json`: Internal Azurite database files.
  - `__queuestorage__/`: Directory for queue storage data.

## Usage

- The files in this folder are automatically generated and managed by Azurite when running local storage emulation.
- Do not edit these files manually.
- This folder can be safely deleted and will be recreated by Azurite as needed.

## Notes

- This folder is excluded from version control via `.gitignore`.
- For more information on Azurite, see the [official documentation](https://github.com/Azure/Azurite).
