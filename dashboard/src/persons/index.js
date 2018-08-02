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
    SelectInput,
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
            <SelectInput source="gender" choices={[
                { id: 'male', name: 'Male' },
                { id: 'female', name: 'Female' },
            ]}/>
            <SelectInput source="type" choices={[
                { id: 'pilgrim', name: 'Pilgrim' },
                { id: 'police', name: 'Police' },
                { id: 'blacklisted', name: 'Blacklisted' },
            ]}/>
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
            <SelectInput source="gender" choices={[
                { id: 'male', name: 'Male' },
                { id: 'female', name: 'Female' },
            ]}/>
            <SelectInput source="type" choices={[
                { id: 'pilgrim', name: 'Pilgrim' },
                { id: 'police', name: 'Police' },
                { id: 'blacklisted', name: 'Blacklisted' },
            ]}/>
            <BooleanInput label="Is Missing?" source="isMissing" />
            <BooleanInput label="Is Captured?" source="found" />
        </SimpleForm>
    </Edit>
);