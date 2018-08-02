import React from 'react';
import { 
    List,
    Datagrid,
    TextField,
    Create,
    SimpleForm,
    SimpleFormIterator,
    TextInput,
    ArrayInput,
    SelectField,
    BooleanInput,
    Edit,
    EditButton,
    DisabledInput,
 } from 'react-admin';

export const PersonList = (props) => (
    <List {...props}>
        <Datagrid>
            <TextField source="id" />
            <TextField source="name" />
            <EditButton />
        </Datagrid>
    </List>
);

export const PersonCreate = (props) => (
    <Create {...props}>
        <SimpleForm>
            <TextInput source="name" />
            <ArrayInput source="images">
                <SimpleFormIterator>
                    <TextInput source="url" />
                </SimpleFormIterator>
            </ArrayInput>
            <SelectField source="gender" choices={[
                { id: 'male', name: 'Male' },
                { id: 'female', name: 'Female' },
            ]} />
            <BooleanInput label="is soldier?" source="isSoldier" />
        </SimpleForm>
    </Create>
);

const PersonTitle = ({ record }) => {
    return <span>Person id: {record ? `"${record.id}"` : ''}</span>;
};

export const PersonEdit = (props) => (
    <Edit title={<PersonTitle />} {...props}>
        <SimpleForm>
            <DisabledInput label="Id" source="id" />
            <TextInput source="name" />
            <ArrayInput source="images">
                <SimpleFormIterator>
                    <TextInput source="url" />
                </SimpleFormIterator>
            </ArrayInput>
            <SelectField source="gender" choices={[
                { id: 'male', name: 'Male' },
                { id: 'female', name: 'Female' },
            ]} />
            <BooleanInput label="is soldier?" source="isSoldier" />
        </SimpleForm>
    </Edit>
);