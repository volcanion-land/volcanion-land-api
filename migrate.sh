#!/bin/bash

# Real Estate Platform - Database Migration Helper Script
# This script helps manage Entity Framework Core migrations

echo "======================================"
echo "Real Estate Platform - Migration Tool"
echo "======================================"
echo ""

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

# Function to print colored output
print_success() {
    echo -e "${GREEN}✓ $1${NC}"
}

print_error() {
    echo -e "${RED}✗ $1${NC}"
}

print_warning() {
    echo -e "${YELLOW}⚠ $1${NC}"
}

# Check if dotnet-ef is installed
check_ef_tools() {
    if ! command -v dotnet-ef &> /dev/null; then
        print_error "dotnet-ef tool is not installed"
        echo "Installing dotnet-ef..."
        dotnet tool install --global dotnet-ef
        print_success "dotnet-ef installed successfully"
    else
        print_success "dotnet-ef is installed"
    fi
}

# Function to create migration
create_migration() {
    echo ""
    read -p "Enter migration name: " migration_name
    
    if [ -z "$migration_name" ]; then
        print_error "Migration name cannot be empty"
        return 1
    fi
    
    echo "Creating migration: $migration_name"
    dotnet ef migrations add "$migration_name"
    
    if [ $? -eq 0 ]; then
        print_success "Migration created successfully"
    else
        print_error "Failed to create migration"
    fi
}

# Function to apply migrations
apply_migrations() {
    echo ""
    print_warning "This will update the database with pending migrations"
    read -p "Continue? (y/n): " confirm
    
    if [ "$confirm" != "y" ]; then
        print_warning "Operation cancelled"
        return 1
    fi
    
    echo "Applying migrations..."
    dotnet ef database update
    
    if [ $? -eq 0 ]; then
        print_success "Database updated successfully"
    else
        print_error "Failed to update database"
    fi
}

# Function to rollback migration
rollback_migration() {
    echo ""
    echo "Available migrations:"
    dotnet ef migrations list
    echo ""
    read -p "Enter migration name to rollback to (or 0 for initial): " target_migration
    
    if [ "$target_migration" = "0" ]; then
        print_warning "This will remove ALL migrations and data"
        read -p "Are you sure? (y/n): " confirm
        if [ "$confirm" != "y" ]; then
            print_warning "Operation cancelled"
            return 1
        fi
        dotnet ef database update 0
    else
        dotnet ef database update "$target_migration"
    fi
    
    if [ $? -eq 0 ]; then
        print_success "Database rolled back successfully"
    else
        print_error "Failed to rollback database"
    fi
}

# Function to remove last migration
remove_last_migration() {
    echo ""
    print_warning "This will remove the last migration (must not be applied to database)"
    read -p "Continue? (y/n): " confirm
    
    if [ "$confirm" != "y" ]; then
        print_warning "Operation cancelled"
        return 1
    fi
    
    dotnet ef migrations remove
    
    if [ $? -eq 0 ]; then
        print_success "Last migration removed successfully"
    else
        print_error "Failed to remove migration"
    fi
}

# Function to generate SQL script
generate_sql_script() {
    echo ""
    read -p "Enter output file name (default: migration.sql): " filename
    filename=${filename:-migration.sql}
    
    echo "Generating SQL script..."
    dotnet ef migrations script -o "$filename"
    
    if [ $? -eq 0 ]; then
        print_success "SQL script generated: $filename"
    else
        print_error "Failed to generate SQL script"
    fi
}

# Function to list migrations
list_migrations() {
    echo ""
    echo "Migrations:"
    dotnet ef migrations list
}

# Function to drop database
drop_database() {
    echo ""
    print_error "⚠️  WARNING: This will DROP the entire database!"
    print_error "All data will be permanently lost!"
    read -p "Type 'DELETE' to confirm: " confirm
    
    if [ "$confirm" != "DELETE" ]; then
        print_warning "Operation cancelled"
        return 1
    fi
    
    dotnet ef database drop --force
    
    if [ $? -eq 0 ]; then
        print_success "Database dropped successfully"
    else
        print_error "Failed to drop database"
    fi
}

# Function to reset database (drop and recreate)
reset_database() {
    echo ""
    print_error "⚠️  WARNING: This will DROP and RECREATE the database!"
    read -p "Type 'RESET' to confirm: " confirm
    
    if [ "$confirm" != "RESET" ]; then
        print_warning "Operation cancelled"
        return 1
    fi
    
    echo "Dropping database..."
    dotnet ef database drop --force
    
    echo "Creating and applying migrations..."
    dotnet ef database update
    
    if [ $? -eq 0 ]; then
        print_success "Database reset successfully"
    else
        print_error "Failed to reset database"
    fi
}

# Main menu
main_menu() {
    while true; do
        echo ""
        echo "======================================"
        echo "Select an option:"
        echo "======================================"
        echo "1. Create new migration"
        echo "2. Apply migrations to database"
        echo "3. Rollback to specific migration"
        echo "4. Remove last migration"
        echo "5. Generate SQL script"
        echo "6. List all migrations"
        echo "7. Drop database"
        echo "8. Reset database (drop & recreate)"
        echo "9. Exit"
        echo "======================================"
        read -p "Enter option (1-9): " option
        
        case $option in
            1) create_migration ;;
            2) apply_migrations ;;
            3) rollback_migration ;;
            4) remove_last_migration ;;
            5) generate_sql_script ;;
            6) list_migrations ;;
            7) drop_database ;;
            8) reset_database ;;
            9) 
                print_success "Goodbye!"
                exit 0
                ;;
            *)
                print_error "Invalid option"
                ;;
        esac
    done
}

# Start script
check_ef_tools
main_menu
